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
        public static IEnumerable<KeyValuePair<TKey, TValue>> InOrder<TKey, TValue>(BSTNode<TKey, TValue> root)
        {
            return Traverse(root, TraversalType.InOrder);
        }

        /// <summary>Traverses the nodes in a binary search tree in pre-order.</summary>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root of the search tree.</param>
        /// <returns>Sequence containing the pre-order traversal of the tree.</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> PreOrder<TKey, TValue>(BSTNode<TKey, TValue> root)
        {
            return Traverse(root, TraversalType.PreOrder);
        }

        /// <summary>Traverses the nodes in a binary search tree in post-order.</summary>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root of the search tree.</param>
        /// <returns>Sequence containing the post-order traversal of the tree.</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> PostOrder<TKey, TValue>(BSTNode<TKey, TValue> root)
        {
            return Traverse(root, TraversalType.PostOrder);
        }

        private static IEnumerable<KeyValuePair<TKey, TValue>> Traverse<TKey, TValue>(BSTNode<TKey, TValue> root, TraversalType type)
        {
            if (root == null)
            {
                yield break;
            }

            var parents = new DynamicStack<NodeTraversal<TKey, TValue>>();
            var current = new NodeTraversal<TKey, TValue>(root);
            parents.Push(current);

            while (parents.Count > 0)
            {
                if (current.VisitedRightSubtree)
                {
                    if (type == TraversalType.PostOrder)
                    {
                        //finished visiting left and right subtrees
                        yield return current.Node.ToKeyValuePair();
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
                        yield return current.Node.ToKeyValuePair();
                    }

                    current.VisitedRightSubtree = true;

                    //visit right subtree if it exists
                    var right = current.Node.Right;
                    if (right != null)
                    {
                        parents.Push(current);
                        current = new NodeTraversal<TKey, TValue>(right);
                    }
                }
                else
                {
                    if (type == TraversalType.PreOrder)
                    {
                        //yield node before visiting subtrees
                        yield return current.Node.ToKeyValuePair();
                    }

                    //visit left subtree
                    current.VisitedLeftSubtree = true;
                    var left = current.Node.Left;

                    if (left != null)
                    {
                        parents.Push(current);
                        current = new NodeTraversal<TKey, TValue>(left);
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

        private class NodeTraversal<TKey, TValue>
        {
            public NodeTraversal(BSTNode<TKey, TValue> node)
            {
                Contract.Requires(node != null);
                this.Node = node;
            }

            public bool VisitedLeftSubtree { get; set; }
            public bool VisitedRightSubtree { get; set; }
            public BSTNode<TKey, TValue> Node { get; private set; }
        }
    }
}
