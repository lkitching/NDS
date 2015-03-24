using System.Collections.Generic;
namespace NDS
{
    /// <summary>Represents a binary tree node with the given key and value.</summary>
    /// <typeparam name="TKey">Key type for this node.</typeparam>
    /// <typeparam name="TValue">Value type for this node.</typeparam>
    public class BSTNode<TKey, TValue>
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

        /// <summary>Returns a key-value pair representing this node.</summary>
        /// <returns>A key-value pair for this node.</returns>
        public KeyValuePair<TKey, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TValue>(this.Key, this.Value);
        }
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
    }
}
