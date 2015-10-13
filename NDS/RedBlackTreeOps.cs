using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    internal static class RedBlackTreeOps
    {
        internal static RedBlackNode<TKey, TValue> ApplyInsert<TKey, TValue>(RedBlackNode<TKey, TValue> root, ArrayList<SearchBranch<RedBlackNode<TKey, TValue>>> searchPath, TKey key, TValue value)
        {
            //if root is null then the tree is empty and this is the first node and therefore the new root
            //NOTE: root is always black
            if(root == null) return new RedBlackNode<TKey,TValue>(key, value, RBNodeColour.Black);

            //NOTE: tree is curently non-empty so the search path must contain at least one item

            //get the parent node and the corresponding branch taken to the child position
            int parentIndex = searchPath.Count - 1;
            var parentBranch = searchPath[parentIndex];
            var parent = parentBranch.Node;

            //insert the new node into the tree
            //node to insert starts out red but may be recoloured
            var newNode = new RedBlackNode<TKey, TValue>(key, value, RBNodeColour.Red) { Parent = parent };
            parentBranch.SetChildNode(newNode);

            var current = newNode;

            while (current != root && current.Colour == RBNodeColour.Red)
            {
                parent = current.Parent;

                //done if the root has been reached
                if (parent == null)
                {
                    //ensure root is always black
                    current.Colour = RBNodeColour.Black;
                    return current;
                }

                //if the parent of the node to insert is black then the tree is still valid and no restructuring is required
                if (parent.Colour == RBNodeColour.Black)
                {
                    return root;
                }

                //NOTE: since the root is always black, the inserted node is not being made a child of the root so
                //it therefore must have a parent and a grandparent node. In addition the parent is red so the 
                //red-black invariant is broken and the tree must be restructured
                var grandParent = parent.Parent;

                //find the uncle node of the current node. This is the sibling of the current node's
                //parent
                var uncle = parent.GetSibling();
                var uncleColour = EffectiveColour(uncle);

                if (uncleColour == RBNodeColour.Red)
                {
                    //if the uncle is red (like the parent) then the grandparent should be black (since the
                    //tree was previously valid). Recolour the parent and uncle nodes to black and the grandparent
                    //to red so the current node is valid with respect to its parent.
                    grandParent.Colour = RBNodeColour.Red;
                    uncle.Colour = parent.Colour = RBNodeColour.Black;

                    //The grandparent may now be invalid with respect to its parent so continue fixing up the tree
                    current = grandParent;
                }
                else
                {
                    //parent is red and uncle is black so rotations are required
                    //there are 4 possible paths from the grandparent to the current node (left, left), (left, right), (right, left), (right, right).
                    //two of these are both oriented in the same direction and require 1 rotation to fix. The other two
                    //require two rotations - one to fix the orientation and the other is the same as the oriented case
                    var gpDir = parent.GetDirectionFromParent();
                    var parentDir = current.GetDirectionFromParent();

                    if (gpDir != parentDir)
                    {
                        //rotate the current node above its parent
                        current.RotateAboveParent();

                        //NOTE: current node is now a child node of its old grandparent and a parent of its old parent
                        //'old' current node reference is no longer required beyond this point so stop tracking it
                        //parent = current;
                    }
                    else
                    {
                        //about to rotate around the parent node
                        current = parent;
                    }

                    //violating nodes are now oriented in the same direction (left, left) or (right, right) and a single rotation
                    //is required. This will make the parent node the parent of the grandparent. The grandparent node must
                    //be black since it previously had a red child (the parent node). The parent node will then have a red child
                    //(the current node) and a black child (the grandparent). The grandparents new children are the uncle node
                    //(known to be black) and the the parent node's other child (if one exists) which must also be black for the 
                    //tree to have been valid. The grandparent can therefore be recoloured to red and the parent node to black
                    //which restores the red-black invariant.
                    current.RotateAboveParent();

                    //recolour (old) parent and grandparent nodes
                    current.Colour = RBNodeColour.Black;
                    grandParent.Colour = RBNodeColour.Red;

                    //NOTE: 'current' node has now been rotated higher up in the tree so continue fixing up
                }
            }

            //ensure root node is black - if we get here then a recolouring reached up to the root node
            //since rotations fix the invariant and return immediately. In this case the tree has not been
            //restructured so the root is unchanged.
            var newRoot = current.FindRoot();
            newRoot.Colour = RBNodeColour.Black;
            return newRoot;
        }

        internal static RedBlackNode<TKey, TValue> ApplyDelete<TKey, TValue>(ArrayList<SearchBranch<RedBlackNode<TKey, TValue>>> searchPath, int matchIndex)
        {
            Contract.Requires(searchPath.Count > 0);
            Contract.Requires(matchIndex >= 0);
            Contract.Requires(matchIndex < searchPath.Count);

            if (searchPath.Count == 1)
            {
                //removing the only node in the tree, so new root is null
                return null;
            }

            var root = searchPath[0].Node;

            //at least two nodes in the tree so find the in-order successor of the node being removed
            //swap the keys and values but leave the colours alone
            //the in-order successor has at most a right-child (NOTE: black leaves are implicit in the representation
            //of the tree).
            //the problem is now how to remove the in-order successor node from the tree while maintaining the red-black
            //invariant
            var removingBranch = searchPath[matchIndex];
            var removingNode = removingBranch.Node;

            var successorBranch = searchPath[searchPath.Count - 1];
            var successor = successorBranch.Node;

            //at least two nodes exist on the search path so get branch taken to reach the successor node from its parent
            var successorParentBranch = searchPath[searchPath.Count - 2];
            var successorParent = successorParentBranch.Node;

            //set successor's key and value in the removing node
            //TODO: make key private and insert new node in place of the removing node?
            removingNode.Key = successor.Key;
            removingNode.Value = successor.Value;

            //NOTE: successor can only have a right child otherwise it would not be the successor.
            var successorChild = successor.Right;

            //remove the successor node from the tree
            //if the removing node is red then its child must be black or the tree is invalid before deletion
            //removing the red node will not affect the black-height of its parent so the removing node can simply
            //be replaced by its child in its parent
            if (successor.Colour == RBNodeColour.Red)
            {
                successorParentBranch.SetChildNode(successorChild);
                return root;
            }

            if (EffectiveColour(successorChild) == RBNodeColour.Red)
            {
                //removing node is black and successor is red
                //move successor in place of the removing node and recolour it to black
                successorParentBranch.SetChildNode(successorChild);
                
                //NOTE: successor child must exist if it is red
                successorChild.Colour = RBNodeColour.Black;
                return root;
            }

            //successor node is a black leaf node
            //it cannot have a black child node or the tree would have been invalid on deletion
            //removing this node will affect the black height of the rest of the tree

            //remove the successor leaf node
            successorParentBranch.SetChildNode(null);

            FixupHole(successorParentBranch.Node, successorParentBranch.Direction);

            return successorParentBranch.Node.FindRoot();
        }

        private static void FixupHole<TKey, TValue>(RedBlackNode<TKey, TValue> parent, BranchDirection dir)
        {
            //if the parent is null then we have reached the root and are done
            if (parent == null) return;

            var sibling = parent.GetChild(dir.OppositeDirection());

            //TODO: ?
            if (sibling == null) return;

            //if the sibling node is red then it must have two black parents
            //this means the parent node must be black (since it has a red child)
            //rotating the sibling above the parent will cause the parent to adopt one of the
            //sibling's children (which are black). The parent's other 'child' will be the hole
            //which is implicitly black.
            //
            if (EffectiveColour(sibling) == RBNodeColour.Red)
            {
                Debug.Assert(parent.Colour == RBNodeColour.Black, "Red parent cannot have a red child");

                //sibling is red so cannot be null
                sibling.RotateAboveParent();

                //swap colours of (old) sibling and parent
                //the hole's relationship to the parent is unchanged but the parent is
                //now red instead of black
                sibling.Colour = RBNodeColour.Black;
                parent.Colour = RBNodeColour.Red;

                //hole now has a red parent and a black sibling (one of the old sibling's old children)
                FixupHole(parent, dir);
            }

            Debug.Assert(EffectiveColour(sibling) == RBNodeColour.Black);

            bool siblingBlackWithBlackChildren = sibling != null && 
                sibling.Colour == RBNodeColour.Black && 
                EffectiveColour(sibling.Left) == RBNodeColour.Black && 
                EffectiveColour(sibling.Right) == RBNodeColour.Black;

            if (parent.Colour == RBNodeColour.Black && siblingBlackWithBlackChildren)
            {
                //if the sibling node is black with black children then it can be recoloured to red
                //this removes a black node from the sibling's subtree to match the one missing from the hole
                //the parent node now has one fewer black node than its sibling so continue fixing up from
                //its parent
                sibling.Colour = RBNodeColour.Red;

                //done if parent is the root
                if (parent.Parent == null) return;
                else
                {
                    FixupHole(parent.Parent, parent.GetDirectionFromParent());
                    return;
                }
            }

            if (parent.Colour == RBNodeColour.Red && siblingBlackWithBlackChildren)
            {
                //if the parent is red and sibling is black then swap their colours
                //this adds a black node to the subtree containing the hole without removing
                //one from the sibling's subtree
                parent.Colour = RBNodeColour.Black;
                sibling.Colour = RBNodeColour.Red;
                return;
            }

            if (dir == BranchDirection.Left && 
                sibling != null && 
                sibling.Colour == RBNodeColour.Black && 
                EffectiveColour(sibling.Left) == RBNodeColour.Red && 
                EffectiveColour(sibling.Right) == RBNodeColour.Black)
            {
                //rotate the sibling's left child above its parent (the sibling node)
                //swap their colours
                //the 'hole' node now has a black sibling
                var siblingLeftChild = sibling.Left;
                siblingLeftChild.RotateAboveParent();
                siblingLeftChild.Colour = RBNodeColour.Black;
                sibling.Colour = RBNodeColour.Red;

                //sibling is now the old left child of the old sibling
                sibling = siblingLeftChild;
            }
            else if (dir == BranchDirection.Right &&
                sibling != null &&
                sibling.Colour == RBNodeColour.Black &&
                EffectiveColour(sibling.Right) == RBNodeColour.Red &&
                EffectiveColour(sibling.Left) == RBNodeColour.Black)
            {
                //NOTE: mirror image of above case
                var siblingRightChild = sibling.Right;
                siblingRightChild.RotateAboveParent();
                siblingRightChild.Colour = RBNodeColour.Black;
                sibling.Colour = RBNodeColour.Red;

                //sibling is now the old right child of the old sibling
                sibling = siblingRightChild;
            }

            //about to rotate the sibling node above the parent
            //the sibling's 
            var siblingChild = sibling.GetChild(dir.OppositeDirection());

            Debug.Assert(siblingChild.Colour == RBNodeColour.Red, "sibling's child should be red");

            //rotate the sibling node above the parent and swap their colours
            //recolour the sibling's child node to black
            //the parent has now moved to be the root of the subtree containing the missing black leaf
            //the parent is now black so the unbalanced subtree has gained a black node and the sibling
            //subtree has gained a black node due to the recolouring
            //the new parent of both subtrees is the same colour as before so the rest of the tree is unaffected
            sibling.RotateAboveParent();
            sibling.Colour = parent.Colour;
            parent.Colour = RBNodeColour.Black;
            siblingChild.Colour = RBNodeColour.Black;
        }

        private static RBNodeColour EffectiveColour<TKey, TValue>(RedBlackNode<TKey, TValue> node)
        {
            //leaf nodes are encoded as null and are black
            return (node == null) ? RBNodeColour.Black : node.Colour;
        }
    }
}
