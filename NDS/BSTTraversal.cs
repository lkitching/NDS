using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Class for traversing binary search trees.</summary>
    public static class BSTTraversal
    {
        /// <summary>Traverses the nodes in a binary search tree in order.</summary>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root of the search tree.</param>
        /// <returns>Sequence containing the in-order traversal of the tree.</returns>
        public static IEnumerable<TNode> InOrder<TNode>(TNode root)
            where TNode : IBinaryNode<TNode>
        {
            return Traverse(root, TraversalType.InOrder);
        }

        /// <summary>Traverses the nodes in a binary search tree in pre-order.</summary>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root of the search tree.</param>
        /// <returns>Sequence containing the pre-order traversal of the tree.</returns>
        public static IEnumerable<TNode> PreOrder<TNode>(TNode root)
            where TNode : IBinaryNode<TNode>
        {
            return Traverse(root, TraversalType.PreOrder);
        }

        /// <summary>Traverses the nodes in a binary search tree in post-order.</summary>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root of the search tree.</param>
        /// <returns>Sequence containing the post-order traversal of the tree.</returns>
        public static IEnumerable<TNode> PostOrder<TNode>(TNode root)
            where TNode : IBinaryNode<TNode>
        {
            return Traverse(root, TraversalType.PostOrder);
        }

        private static IEnumerable<TNode> Traverse<TNode>(TNode root, TraversalType type)
            where TNode : IBinaryNode<TNode>
        {
            if (root == null)
            {
                yield break;
            }

            var parents = new DynamicStack<NodeTraversal<TNode>>();
            var current = new NodeTraversal<TNode>(root);
            parents.Push(current);

            while (parents.Count > 0)
            {
                if (current.VisitedRightSubtree)
                {
                    if (type == TraversalType.PostOrder)
                    {
                        //finished visiting left and right subtrees
                        yield return current.Node;
                    }

                    Debug.Assert(current.VisitedLeftSubtree, "Visited right subtree before left");

                    //move back to parent
                    current = parents.Pop();
                }
                else if (current.VisitedLeftSubtree)
                {
                    if (type == TraversalType.InOrder)
                    {
                        //yield this node after visiting left subtree
                        yield return current.Node;
                    }

                    current.VisitedRightSubtree = true;

                    //visit right subtree if it exists
                    var right = current.Node.Right;
                    if (right != null)
                    {
                        parents.Push(current);
                        current = new NodeTraversal<TNode>(right);
                    }
                }
                else
                {
                    if (type == TraversalType.PreOrder)
                    {
                        //yield node before visiting subtrees
                        yield return current.Node;
                    }

                    //visit left subtree
                    current.VisitedLeftSubtree = true;
                    var left = current.Node.Left;

                    if (left != null)
                    {
                        parents.Push(current);
                        current = new NodeTraversal<TNode>(left);
                    }
                }
            }
        }

        private enum TraversalType
        {
            InOrder = 0,
            PreOrder = 1,
            PostOrder = 2
        }

        private class NodeTraversal<TNode>
            where TNode : IBinaryNode<TNode>
        {
            public NodeTraversal(TNode node)
            {
                Contract.Requires(node != null);
                this.Node = node;
            }

            public bool VisitedLeftSubtree { get; set; }
            public bool VisitedRightSubtree { get; set; }
            public TNode Node { get; private set; }
        }
    }
}
