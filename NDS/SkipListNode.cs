using System;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents a node in a skip list.</summary>
    /// <typeparam name="TKey">The key type for this node.</typeparam>
    /// <typeparam name="TValue">The type of value for this node.</typeparam>
    public class SkipListNode<TKey, TValue> : IHasNext<SkipListNode<TKey, TValue>>
    {
        private readonly SkipListNode<TKey, TValue>[] links;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="key">The key for this node.</param>
        /// <param name="value">The initial value for this node.</param>
        /// <param name="level">The number of links to successor nodes to be stored in this list.</param>
        public SkipListNode(TKey key, TValue value, int level)
        {
            Contract.Requires(level > 0);

            this.Key = key;
            this.Value = value;
            this.Level = level;
            this.links = new SkipListNode<TKey, TValue>[level];
        }

        /// <summary>Gets the key for this node.</summary>
        public TKey Key { get; private set; }

        /// <summary>Gets/sets the value associated with this node.</summary>
        public TValue Value { get; set; }

        /// <summary>Gets the number of links to successor nodes stored in this node.</summary>
        public int Level { get; private set; }

        /// <summary>Gets/sets the successor node at the given level.</summary>
        /// <param name="index">The level of the list to find the successor for.</param>
        /// <returns>The successor node at the given level, or null if none exists.</returns>
        public SkipListNode<TKey, TValue> this[int index]
        {
            get
            {
                this.GuardIndex(index);
                return this.links[index];
            }
            set
            {
                this.GuardIndex(index);
                this.links[index] = value;
            }
        }

        /// <summary>Gets the direct successor node to this node.</summary>
        public SkipListNode<TKey, TValue> Next
        {
            get { return this.links[0]; }
        }

        private void GuardIndex(int index)
        {
            if (index < 0 || index >= this.links.Length)
            {
                var msg = string.Format("Index must be in the range [0, {0})", this.links.Length);
                throw new ArgumentOutOfRangeException("index", msg);
            }
        }
    }
}
