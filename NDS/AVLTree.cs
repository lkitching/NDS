using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    public class AVLTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IComparer<TKey> keyComparer;
        private int count;
        private AVLNode<TKey, TValue> root;

        public AVLTree() : this(Comparer<TKey>.Default) { }
        public AVLTree(IComparer<TKey> comp)
        {
            Contract.Requires(comp != null);
            this.keyComparer = comp;
        }

        public Maybe<TValue> Get(TKey key)
        {
            return BSTNode.Get(this.root, key, this.keyComparer);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (this.TryInsertRoot(key, value)) return true;

            var insertContext = FindInsertLocation(key);
            if (insertContext.Found) return false;
            else
            {
                ApplyInsert(key, value, insertContext);
                return true;
            }
        }

        public void Assoc(TKey key, TValue value)
        {
            if (!this.TryInsertRoot(key, value))
            {
                var insertContext = FindInsertLocation(key);
                if (insertContext.Found)
                {
                    insertContext.Parent.Value = value;
                }
                else
                {
                    this.ApplyInsert(key, value, insertContext);
                }
            }
        }

        private bool TryInsertRoot(TKey key, TValue value)
        {
            if (this.count == 0)
            {
                Debug.Assert(this.root == null);
                this.root = new AVLNode<TKey, TValue>(key, value);
                this.count++;
                return true;
            }
            else return false;
        }

        public bool Delete(TKey key)
        {
            //find the node to remove
            var context = FindNodeToDelete(key, this.root, this.keyComparer);
            if (!context.Found) return false;
            
            //find in-order successor of removed node and update search path to include
            //all parents of removed leaf
            var searchPath = RemoveNode(context);

            //fix tree up through search path to removed node
            FixDeleteBalanceFactors(searchPath);

            this.count--;
            return true;
        }

        private static void SetChild(SearchBranch branch, AVLNode<TKey, TValue> newChild)
        {
            var parentNode = branch.Node;

            if (branch.Direction == SearchDirection.Left)
            {
                parentNode.Left = newChild;
            }
            else
            {
                parentNode.Right = newChild;
            }
        }

        private IStack<SearchBranch> RemoveNode(DeleteContext context)
        {
            Debug.Assert(context.Found, "Cannot remove node from unsuccessful search");

            var toRemove = context.MatchingNode;
            var searchPath = context.SearchPath;

            if (context.SearchPath.Count == 0)
            {
                //root node is being removed
                Debug.Assert(toRemove == this.root, "Should be removing root if search path is empty");
                this.root = null;
                return searchPath;
            }

            var parentBranch = searchPath.Peek();

            //as with deleting from a regular binary search tree, the node to be removed
            //should be swapped with its in-order successor and the leaf node deleted.
            if (toRemove.Right == null)
            {
                //node has no right subtree so it can be replaced by its left subtree in the parent
                SetChild(parentBranch, toRemove.Left);
                return searchPath;
            }

            var childNode = toRemove.Right;

            if (childNode.Left == null)
            {
                //right child has no left subtree
                //swap toRemove with the right child and add child to the stack of nodes to remove
                childNode.Left = toRemove.Left;
                childNode.BalanceFactor = toRemove.BalanceFactor;
                searchPath.Push(new SearchBranch(childNode, SearchDirection.Right));
                SetChild(parentBranch, childNode);
            }
            else
            {
                //the current search path extends to the parent of the node to be removed
                //root ... parent, toRemove
                //the in-order successor of toRemove is the left-most node in the subtree rooted by the 
                //right-child of toRemove. If the path to this node is [childNode ... p, inOrderSucc]
                //then the search path must be extended to be [root ... parent, inOrderSucc, childNode ... p]
                //inOrderSucc has not yet been found so add the nodes [childNode ... p] into a list so they can be 
                //pushed on the stack after the left-most leaf node (inOrderSucc) has been found
                var current = childNode;
                var leftMost = childNode.Left;
                var parentQueue = new ArrayList<SearchBranch>();

                do
                {
                    parentQueue.Add(new SearchBranch(current, SearchDirection.Left));
                    current = leftMost;
                    leftMost = leftMost.Left;
                } while (leftMost.Left != null);

                //swap toRemove with its in-order successor (leftMost)
                //current is the parent of leftMost
                //    current
                //   /
                //  leftMost
                //          \
                //           r
                //replace leftMost with r as the left-child of current
                //set left and right children of leftMost to the left and right children
                //of toRemove
                //set balance factor of leftMost to the current factor of toRemove since they are being swapped
                current.Left = leftMost.Right;
                leftMost.Left = toRemove.Left;
                leftMost.Right = childNode;
                leftMost.BalanceFactor = toRemove.BalanceFactor;

                //update toRemove's parent to its in-order successor
                SetChild(parentBranch, leftMost);

                //update the search path
                //first push path to childNode (right)
                searchPath.Push(new SearchBranch(leftMost, SearchDirection.Right));

                foreach(var branch in parentQueue)
                {
                    searchPath.Push(branch);
                }
            }
            
            //Removing the leaf could cause the balance factors of its ancestors on the search path to be modified
            //so the existing search path needs to be extended
            return searchPath;
        }

        private void FixDeleteBalanceFactors(IStack<SearchBranch> searchPath)
        {
            while (searchPath.Count > 0)
            {
                var currentBranch = searchPath.Pop();
                var currentNode = currentBranch.Node;

                Debug.Assert(currentNode.BalanceFactor >= -1 && currentNode.BalanceFactor <= 1, "Unexpected balance factor for node on search path");

                //if the current balance factor is 0 then after the deletion, the overall height of the subtree rooted at
                //curentNode will not be changed. In this case no further balance factors further up the search path
                //need to be modified
                bool heightUnchanging = currentNode.BalanceFactor == 0;

                //balance factor is decreasing if deletion was in the left subtree, otherwise increasing
                int change = currentBranch.Direction == SearchDirection.Left ? (-1) : 1;
                currentNode.BalanceFactor += change;

                //need to rebalance if balance factor has violated the invariant
                if (currentNode.BalanceFactor == 2 || currentNode.BalanceFactor == -2)
                {
                    var newNode = RebalanceAt(currentNode);
                    if (searchPath.Count > 0)
                    {
                        var parentBranch = searchPath.Peek();
                        SetChild(parentBranch, newNode);
                    }
                    else
                    {
                        Debug.Assert(currentNode == this.root, "Should be at root if search path empty");
                        this.root = newNode;
                    }
                }
            }
        }

        private static DeleteContext FindNodeToDelete(TKey key, AVLNode<TKey, TValue> root, IComparer<TKey> keyComparer)
        {
            var current = root;
            var searchPath = new DynamicStack<SearchBranch>();

            while (current != null)
            {
                var c = keyComparer.Compare(key, current.Key);
                if (c < 0)
                {
                    searchPath.Push(new SearchBranch(current, SearchDirection.Left));
                    current = current.Left;
                }
                if (c == 0)
                {
                    return new DeleteContext(searchPath, current);
                }
                else
                {
                    searchPath.Push(new SearchBranch(current, SearchDirection.Right));
                    current = current.Right;
                }
            }

            return new DeleteContext(searchPath, null);
        }

        private struct DeleteContext
        {
            public DeleteContext(IStack<SearchBranch> searchPath, AVLNode<TKey, TValue> matchingNode)
                : this()
            {
                this.SearchPath = searchPath;
                this.MatchingNode = matchingNode;
            }

            public IStack<SearchBranch> SearchPath { get; private set; }
            public AVLNode<TKey, TValue> MatchingNode { get; private set; }

            public bool Found
            {
                get { return this.MatchingNode != null; }
            }
        }

        private InsertContext FindInsertLocation(TKey key)
        {
            Debug.Assert(this.root != null);

            var current = this.root;
            int? unbalancedNodeDepth = null;
            int currentDepth = 0;
            var searchPath = new ArrayList<SearchBranch>(20);

            while (current != null)
            {
                if (current.BalanceFactor != 0)
                {
                    unbalancedNodeDepth = currentDepth;
                }

                int c = this.keyComparer.Compare(key, current.Key);
                if (c == 0)
                {
                    //key already exists so value can just be updated
                    searchPath.Add(new SearchBranch(current, (SearchDirection)(-1)));
                    return new InsertContext(searchPath, true, unbalancedNodeDepth);
                }
                else
                {
                    var direction = c < 0 ? SearchDirection.Left : SearchDirection.Right;
                    searchPath.Add(new SearchBranch(current, direction));
                    current = c < 0 ? current.Left : current.Right;
                }

                currentDepth++;
            }

            return new InsertContext(searchPath, false, unbalancedNodeDepth);
        }

        private void InsertNewLeaf(TKey key, TValue value, InsertContext context)
        {
            var newNodeBranch = context.SearchPath[context.SearchPath.Count - 1];
            var parent = newNodeBranch.Node;
            var newNode = new AVLNode<TKey, TValue>(key, value);

            if(newNodeBranch.Direction == SearchDirection.Left)
            {
                Debug.Assert(parent.Left == null, "Should insert leaf");
                parent.Left = newNode;
            }
            else
            {
                Debug.Assert(newNodeBranch.Direction == SearchDirection.Right, "Invalid search direction when inserting");
                Debug.Assert(parent.Right == null, "Should insert leaf");
                parent.Right = newNode;
            }
        }
        private void ApplyInsert(TKey key, TValue value, InsertContext context)
        {
            //insert new node at the end of the search path
            InsertNewLeaf(key, value, context);

            //the effects of the insert will only propagate as far as the node at
            //UnbalancedPathRootIndex on the search path. This is the deepest node in the
            //search path with a non-zero balance factor. All nodes with 0 balance factors
            //on the search path will have their balance factors updated to +/-1. The
            //node at UnbalancedPathRootIndex will have its balance factor updated to either
            //0 or +/-2. If it changes to 0 then nothing more need to be done. If it becomes
            //+/-2 then the AVL invariant is violated and 1 or 2 rotations are required to restore
            //it. In either case the height of the subtree rooted by the node at UnbalancedPathRootIndex
            //remains unchanged, so the balance factors of all its ancestors are also unchanged.

            //update balance factors from UnbalancedPathRootIndex down to the leaf node being 
            //inserted into. If UnbalancedPathRootIndex is null there no nodes in the search
            //path with a non-zero balance factor so the updates apply all the way up to the 
            //root. No rebalancing is required in this case.

            for (int i = context.UnbalancedPathRootIndex ?? 0; i < context.SearchPath.Count; ++i)
            {
                var currentBranch = context.SearchPath[i];
                
                //balance factor increases by 1 if inserting into left subtree, otherwise decreases by 1
                var change = currentBranch.Direction == SearchDirection.Left ? 1 : (-1);
                currentBranch.Node.BalanceFactor += change;
            }

            //rebalancing required if the balance root's balance factor has changed to 2 or -2
            if (context.UnbalancedPathRootIndex.HasValue)
            {
                int balanceIndex = context.UnbalancedPathRootIndex.Value;
                var balanceBranch = context.SearchPath[balanceIndex];
                var balanceRoot = balanceBranch.Node;

                if (balanceRoot.BalanceFactor != 0)
                {
                    Debug.Assert(balanceRoot.BalanceFactor == 2 || balanceRoot.BalanceFactor == -2, "Unexpected balance factor for balance root");

                    //Need to rebalance at the balance root
                    //there are 3 nodes involved in the re-balancing: the balance root, its parent and
                    //its child. There must be a child node on the search path since a rebalancing should
                    //only be required when inserting into an already-unbalanced subtree
                    //if the parent of the inserted node is was a leaf then it had a balance factor of 0
                    //so cannot be the balance root. If the parent of the inserted node had a non-zero balance
                    //factor then it must have been inserted into the other subtree, meaning it would
                    //now have a 0 balance factor. Therefore the balance root must be no lower in the tree
                    //than the parent of the node being inserted into
                    var newRebalancedRoot = RebalanceAt(balanceRoot);

                    if (balanceIndex > 0)
                    {
                        //update child node in the parent of the old rebalance root
                        var parentBranch = context.SearchPath[balanceIndex - 1];
                        var parentNode = parentBranch.Node;
                        if (parentBranch.Direction == SearchDirection.Left)
                        {
                            parentNode.Left = newRebalancedRoot;
                        }
                        else
                        {
                            Debug.Assert(parentBranch.Direction == SearchDirection.Right, "");
                            parentNode.Right = newRebalancedRoot;
                        }
                    }
                    else
                    {
                        //rotation was performed at root so it has changed
                        this.root = newRebalancedRoot;
                    }
                }
            }

            this.count++;
        }

        private AVLNode<TKey, TValue> RebalanceAt(AVLNode<TKey, TValue> rootNode)
        {
            Debug.Assert(rootNode.BalanceFactor == 2 || rootNode.BalanceFactor == -2, "Rebalance not required for node");

            if (rootNode.BalanceFactor == 2)
            {
                var childNode = rootNode.Left;
                Debug.Assert(childNode != null, "Left chlid required for node with +2 balance factor");

                Debug.Assert(childNode.BalanceFactor == 1 || childNode.BalanceFactor == -1, "Child of rebalanced node must have non-zero balance");

                if (childNode.BalanceFactor == 1)
                {
                    //left-left case
                    Debug.Assert(rootNode.BalanceFactor == 2, "Unexpected balance factor for root in left-left case");
                    Debug.Assert(childNode.BalanceFactor == 1, "Unexpected balance factor for child not in left-left case");

                    //requires a single right rotation around the child node so it becomes the root
                    //after rotation the balance factors of both nodes is 0
                    //child's right child becomes root's left child
                    //root becomes child's right-child
                    rootNode.Left = childNode.Right;
                    childNode.Right = rootNode;
                    rootNode.BalanceFactor = 0;
                    childNode.BalanceFactor = 0;

                    return childNode;
                }
                else
                {
                    var x = childNode.Right;
                    Debug.Assert(x.BalanceFactor >= -1 && x.BalanceFactor <= 1, "Unexpected balance factor for child node's right child");

                    //requires a double rotation - a left rotation around the child followed by a
                    //right rotation to bring x up to the root
                    //       r               x
                    //      / \             / \
                    //     c   b   =>      c   r
                    //    / \             / \ / \
                    //   a   x           a   d   b
                    //  /
                    // x's left child becomes the child node's right child
                    // x's right child becomes the root node's left child
                    // the child node becomes x's left child
                    // the root node becomes x's right child
                    childNode.Right = x.Left;
                    rootNode.Left = x.Right;
                    x.Left = childNode;
                    x.Right = rootNode;

                    //update balance factors. The balance factor of x will be 0.
                    //the balance factors for the child and root nodes depend on the current balance factor of
                    //x. If it is 0 then x is the newly-inserted node
                    if (x.BalanceFactor == 0)
                    {
                        //child node's right subtree is now empty when it used to contain x so it's balance has increased by 1 to 0
                        //root node's left subtree is now empty when it used to be +2
                        //NOTE: root node's right child must be null
                        Debug.Assert(rootNode.Right == null);
                        childNode.BalanceFactor = 0;
                        rootNode.BalanceFactor = 0;
                    }
                    else if (x.BalanceFactor == 1)
                    {
                        //x's left subtree was larger than the right
                        //left subtree moves to child so it's balance increases to 0
                        //right subtree moves to root
                        //root node has lost 2 due to rotations and 1 due to inheriting the smaller subtree of x
                        childNode.BalanceFactor = 0;
                        rootNode.BalanceFactor = -1;
                    }
                    else
                    {
                        //x's right subtree was larger than the left
                        //x's left subtree is now right-child of child
                        //child has lost x and now has shorter tree in right child so it's balance has increased by 2 to +1
                        //root node has lost 2 due to rotations so it's balance is now 0
                        childNode.BalanceFactor = 1;
                        rootNode.BalanceFactor = 0;
                    }

                    x.BalanceFactor = 0;
                    return x;
                }
            }
            else
            {
                var childNode = rootNode.Right;
                Debug.Assert(childNode != null, "Right child required for node with -2 balance");
                Debug.Assert(childNode.BalanceFactor == 1 || childNode.BalanceFactor == -1, "Child of rebalanced node must have non-zero balance factor");

                if(childNode.BalanceFactor == 1)
                {
                    //right-left case
                    var x = childNode.Left;
                    Debug.Assert(x.BalanceFactor >= -1 && x.BalanceFactor <= 1, "Unexpected balance factor for child node's left child");

                    //requires a double rotation to rebalance - a right rotation around the child node followed by
                    //a left rotation around x. This brings x to the root and makes the root node and child node its
                    //left and right children respectively
                    //x's left child becomes the root node's right child
                    //x's right child becomes the child node's left child
                    rootNode.Right = x.Left;
                    childNode.Left = x.Right;
                    x.Left = rootNode;
                    x.Right = childNode;

                    //update balance factors. The balance factor of x will become 0
                    //the new balance factors for the child and root depend on the balance factor for x
                    //if it is 0 then x is the newly-inserted node and both its subtrees should be empty
                    if (x.BalanceFactor == 0)
                    {
                        childNode.BalanceFactor = 0;
                        rootNode.BalanceFactor = 0;
                    }
                    else if (x.BalanceFactor == 1)
                    {
                        //x's left subtree was taller than its right
                        //root node has gained 2 due to rotations and 1 due to inheriting x's left child
                        //child has lost one due to moving x and one due to inheriting x's right child
                        childNode.BalanceFactor = -1;
                        rootNode.BalanceFactor = 0;
                    }
                    else
                    {
                        //x's right subtree was taller than its left
                        //child node has lost 1 due to 
                        childNode.BalanceFactor = 0;
                        rootNode.BalanceFactor = 1;
                    }

                    x.BalanceFactor = 0;
                    return x;
                }
                else
                {
                    //right-right case

                    //requires a single left rotation around the child node so it becomes the root
                    //after rotation the balance factors of both nodes is 0
                    //child's left child becomes root's right child
                    //root becomes child's left child
                    rootNode.Right = childNode.Left;
                    childNode.Left = rootNode;
                    rootNode.BalanceFactor = 0;
                    childNode.BalanceFactor = 0;

                    return childNode;
                }
            }
        }

        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        public int Count
        {
            get { return this.count; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return BSTTraversal.InOrder(this.root).Select(n => n.ToKeyValuePair()).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private enum SearchDirection { Left, Right }
        private struct SearchBranch
        {
            public SearchBranch(AVLNode<TKey, TValue> node, SearchDirection direction) : this()
            {
                this.Node = node;
                this.Direction = direction;
            }

            public AVLNode<TKey, TValue> Node { get; private set; }
            public SearchDirection Direction { get; private set; }
        }

        private struct InsertContext
        {
            public InsertContext(ArrayList<SearchBranch> searchPath, bool found, int? unbalancedPathRootIndex) 
                : this()
            {
                this.SearchPath = searchPath;
                this.UnbalancedPathRootIndex = unbalancedPathRootIndex;
                this.Found = found;
            }

            public ArrayList<SearchBranch> SearchPath { get; private set; }
            public int? UnbalancedPathRootIndex { get; private set; }

            public AVLNode<TKey, TValue> Parent
            {
                get { return this.SearchPath[this.SearchPath.Count - 1].Node; }
            }

            public SearchDirection ParentInsertLocation
            {
                get { return this.SearchPath[this.SearchPath.Count - 1].Direction; }
            }

            public bool Found { get; private set; }
        }
    }
}
