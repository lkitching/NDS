using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents a node in a binary search tree.</summary>
    /// <typeparam name="TNode">Type of child nodes.</typeparam>
    /// <typeparam name="TKey">Key type for the node.</typeparam>
    /// <typeparam name="TValue">Value type for the node.</typeparam>
    public interface IBSTNode<TNode, out TKey, out TValue> : IBinaryNode<TNode>
        where TNode : IBSTNode<TNode, TKey, TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
    }

    /// <summary>Represents a binary tree node with the given key and value.</summary>
    /// <typeparam name="TKey">Key type for this node.</typeparam>
    /// <typeparam name="TValue">Value type for this node.</typeparam>
    public class BSTNode<TKey, TValue> : IBSTNode<BSTNode<TKey, TValue>, TKey, TValue>
    {
        public BSTNode(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>Gets the key for this node.</summary>
        public TKey Key { get; private set; }

        /// <summary>Gets and sets the value for this node.</summary>
        public TValue Value { get; set; }

        /// <summary>Gets and sets the left child of this node. Can be null.</summary>
        public BSTNode<TKey, TValue> Left { get; set; }

        /// <summary>Gets and sets the right child of this node. Can be null.</summary>
        public BSTNode<TKey, TValue> Right { get; set; }
    }

    public class SizedBSTNode<TKey, TValue> : IBSTNode<SizedBSTNode<TKey, TValue>, TKey, TValue>
    {
        /// <summary>Creates a new node with the given key and value.</summary>
        /// <param name="key">The key for this node.</param>
        /// <param name="value">The value for this node.</param>
        public SizedBSTNode(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>Gets the key for this node.</summary>
        public TKey Key { get; private set; }

        /// <summary>Gets/sets the value for this node.</summary>
        public TValue Value { get; set; }

        /// <summary>Gets/sets the left child of this node.</summary>
        public SizedBSTNode<TKey, TValue> Left { get; set; }

        /// <summary>Gets/sets the right child of this node.</summary>
        public SizedBSTNode<TKey, TValue> Right { get; set; }

        /// summary>Gets/sets the number of nodes in the tree rooted by this node.</summary>
        public int Count { get; set; }
    }

    /// <summary>Utility class for binary search tree nodes.</summary>
    public static class BSTNode
    {
        /// <summary>Creates a new <see cref="BSTNode{TKey, TValue}"/>.</summary>
        /// <typeparam name="TKey">Key type for the node.</typeparam>
        /// <typeparam name="TValue">Value type for the node.</typeparam>
        /// <param name="key">Key for the created node.</param>
        /// <param name="value">Value for the created node.</param>
        /// <param name="left">The left child for this node.</param>
        /// <param name="right">The right child for this node.</param>
        /// <returns>A new <see cref="BSTNode{TKey, TValue}"/> with the given key and value.</returns>
        public static BSTNode<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value, BSTNode<TKey, TValue> left = null, BSTNode<TKey, TValue> right = null)
        {
            return new BSTNode<TKey, TValue>(key, value) { Left = left, Right = right };
        }

        /// <summary>Tries to find the value associated with the given key in a binary search tree.</summary>
        /// <typeparam name="TNode"></typeparam>
        /// <typeparam name="TKey">Key type of the search tree.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="root">The root node of the tree. This can be null if the tree is empty.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="keyComparer">Comparer for keys in the search tree.</param>
        /// <returns>The value associated with <paramref name="key"/> in the tree with root <paramref name="root"/> if it exists, otherwise None.</returns>
        public static Maybe<TValue> Get<TNode, TKey, TValue>(IBSTNode<TNode, TKey, TValue> root, TKey key, IComparer<TKey> keyComparer)
            where TNode : IBSTNode<TNode, TKey, TValue>
        {
            Contract.Requires(keyComparer != null);

            var current = root;
            while (current != null)
            {
                switch (current.FindKey(key, keyComparer))
                {
                    case BSTComparisonResult.This: return Maybe.Some(current.Value);
                    case BSTComparisonResult.Left:
                        current = current.Left;
                        break;
                    default:
                        current = current.Right;
                        break;
                }
            }

            return Maybe.None<TValue>();
        }

        /// <summary>Rotates a binary tree node to the left.</summary>
        /// <typeparam name="TNode">Node type in the tree.</typeparam>
        /// <param name="parent">The parent node to rotate left with respect to its right child.</param>
        /// <returns>The new parent node.</returns>
        public static TNode RotateLeftFromParent<TNode>(TNode parent)
            where TNode : IBinaryNode<TNode>
        {
            if (parent.Right != null)
            {
                var newRoot = parent.Right;
                parent.Right = newRoot.Left;
                newRoot.Left = parent;
                return newRoot;
            }
            else { return parent; }
        }

        /// <summary>Rotates left around the given node.</summary>
        /// <typeparam name="TNode">The type of the nodes in the tree.</typeparam>
        /// <param name="node">The node to rotate around.</param>
        public static void RotateLeft<TNode>(this TNode node)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            Rotate(node, BranchDirection.Left);
        }

        /// <summary>Rotates a binary tree node to the right.</summary>
        /// <typeparam name="TNode">Node type in the tree.</typeparam>
        /// <param name="parent">The parent node to rotate right with respect with its left child.</param>
        /// <returns>The new parent node.</returns>
        public static TNode RotateRightFromParent<TNode>(TNode parent)
            where TNode : IBinaryNode<TNode>
        {
            Contract.Requires(parent != null);

            if (parent.Left != null)
            {
                var newRoot = parent.Left;
                parent.Left = newRoot.Right;
                newRoot.Right = parent;
                return newRoot;
            }
            else { return parent; }
        }

        /// <summary>Rotates in the given direction around the given parent node.</summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="parent">The parent node to rotate around.</param>
        /// <param name="direction">The direction to rotate in.</param>
        /// <returns>The new parent of <paramref name="parent"/> after the rotation.</returns>
        internal static TNode RotateFromParent<TNode>(TNode parent, BranchDirection direction)
            where TNode : IBinaryNode<TNode>
        {
            return direction == BranchDirection.Left
                ? RotateLeftFromParent(parent)
                : RotateRightFromParent(parent);
        }

        /// <summary>Rotates right around the given node.</summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="node">The node to rotate around.</param>
        public static void RotateRight<TNode>(this TNode node)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            Rotate(node, BranchDirection.Right);
        }

        internal static void Rotate<TNode>(this TNode node, BranchDirection direction)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            Contract.Requires(node != null);
            Contract.Requires(node.Parent != null);

            var parent = node.Parent;
            var gp = parent.Parent;

            //the child in the rotating direction will be moved to the current parent
            var movingChild = node.GetChild(direction);

            //save which child of grandparent the current parent is if one exists
            var parentChildDir = gp == null ? (BranchDirection?)null : parent.GetDirectionFromParent();

            RotateFromParent(node.Parent, direction);

            parent.Parent = node;
            node.Parent = gp;
            if (movingChild != null)
            {
                //moved child is now the opposite child of the old parent
                movingChild.Parent = parent;
            }

            //node is now the child of its old grandparent node (if one exists)
            //update the grandparent to point to node instead of its old parent
            if (parentChildDir.HasValue)
            {
                gp.SetChild(parentChildDir.Value, node);
            }
        }

        public static Tuple<bool, BSTNode<TKey, TValue>> Delete<TKey, TValue>(BSTNode<TKey, TValue> root, TKey key, IComparer<TKey> keyComparer)
        {
            Contract.Requires(keyComparer != null);

            BSTNode<TKey, TValue> current = root;
            BSTNode<TKey, TValue> parent = null;

            while (current != null)
            {
                var c = current.FindKey(key, keyComparer);
                if (c == BSTComparisonResult.This)
                {
                    var newRoot = BSTNode.DeleteRoot(current);
                    if (parent == null)
                    {
                        Debug.Assert(current == root);
                    }
                    else if (parent.Left == current) { parent.Left = newRoot; }
                    else
                    {
                        Debug.Assert(parent.Right == current);
                        parent.Right = newRoot;
                    }

                    return Tuple.Create(true, parent);
                }
                else if (c == BSTComparisonResult.Left)
                {
                    //search left subtree
                    parent = current;
                    current = current.Left;
                }
                else
                {
                    //search right subtree
                    parent = current;
                    current = current.Right;
                }
            }

            //key not found so root is unchanged
            return Tuple.Create(false, root);
        }

        /// <summary>Deletes the given root node of a binary search tree, merges the two child trees and returns the new root.</summary>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root node to delete.</param>
        /// <returns>The new root node for the tree.</returns>
        public static BSTNode<TKey, TValue> DeleteRoot<TKey, TValue>(BSTNode<TKey, TValue> root)
        {
            Contract.Requires(root != null);

            //if left subtree is empty then new root is right subtree
            if (root.Left == null) return root.Right;

            //if right subtree is empty then new root is left subtree
            if (root.Right == null) return root.Left;

            //Both subtrees are non-empty. The smallest element greater than the current root is the left-most
            //node in the right subtree - this should become the new root. The right-subtree of this node
            //should become the new left subtree of that node's parent. There is no left subtree since it is
            //the left-most node in the right subtree.
            var parent = root.Right;
            var current = parent.Left;

            while (current != null)
            {
                parent = current;
                current = current.Left;
            }

            var newRoot = new BSTNode<TKey, TValue>(current.Key, current.Value) { Left = root.Left, Right = root.Right };
            parent.Left = current.Right;
            return newRoot;
        }

        /// <summary>Returns a key-value pair representing a binary search tree node.</summary>
        /// <param name="node">The node.</param>
        /// <returns>A key-value pair for <paramref name="node"/>.</returns>
        public static KeyValuePair<TKey, TValue> ToKeyValuePair<TNode, TKey, TValue>(this IBSTNode<TNode, TKey, TValue> node)
            where TNode : IBSTNode<TNode, TKey, TValue>
        {
            return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        }
    }
}
