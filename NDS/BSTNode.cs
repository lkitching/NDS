using System.Collections.Generic;
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
                int c = keyComparer.Compare(key, current.Key);
                if (c == 0)
                {
                    return Maybe.Some(current.Value);
                }
                else if (c < 0)
                {
                    //search left subtree
                    current = current.Left;
                }
                else
                {
                    //search right subtree
                    current = current.Right;
                }
            }

            return Maybe.None<TValue>();
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
