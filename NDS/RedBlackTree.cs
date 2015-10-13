using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>Red-black tree.</summary>
    /// <typeparam name="TKey">The key type for this tree.</typeparam>
    /// <typeparam name="TValue">The value type for this tree.</typeparam>
    public class RedBlackTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IComparer<TKey> keyComparer;
        private RedBlackNode<TKey, TValue> root;
        private int count = 0;

        /// <summary>Creates a new empty tree with the given comparer for keys.</summary>
        /// <param name="keyComparer">Comparer for the keys in this tree.</param>
        public RedBlackTree(IComparer<TKey> keyComparer)
        {
            Contract.Requires(keyComparer != null);
            this.keyComparer = keyComparer;
        }

        /// <see cref="IMap{TKey, TValue}.Get"/>
        public Maybe<TValue> Get(TKey key)
        {
            return BSTNode.Get(this.root, key, this.keyComparer);
        }

        /// <see cref="IMap{TKey, TValue}.TryAdd"/>
        public bool TryAdd(TKey key, TValue value)
        {
            var context = BSTSearch.SearchFor<RedBlackNode<TKey, TValue>, TKey, TValue>(this.root, key, this.keyComparer);
            if (context.Found) return false;
            else
            {
                ApplyInsert(context, key, value);
                return true;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            var context = BSTSearch.SearchFor<RedBlackNode<TKey, TValue>, TKey, TValue>(this.root, key, this.keyComparer);
            if (context.Found)
            {
                context.MatchingNode.Value = value;
            }
            else
            {
                ApplyInsert(context, key, value);
            }
        }

        private void ApplyInsert(IBSTSearchContext<RedBlackNode<TKey, TValue>> context, TKey key, TValue value)
        {
            this.root = RedBlackTreeOps.ApplyInsert(this.root, context.SearchPath, key, value);
            this.count++;
        }

        /// <see cref="IMap{TKey, TValue}.Delete"/>
        public bool Delete(TKey key)
        {
            var context = BSTSearch.SearchForDelete<RedBlackNode<TKey, TValue>, TKey, TValue>(this.root, key, this.keyComparer);
            if (context.MatchPathIndex.HasValue)
            {
                this.root = RedBlackTreeOps.ApplyDelete(context.SearchPath, context.MatchPathIndex.Value);
                this.count--;
                return true;
            }
            else
            {
                //key not found
                return false;
            }
        }

        /// <summary>Removes all the nodes from this tree.</summary>
        public void Clear()
        {
            this.count = 0;
            this.root = null;
        }

        /// <summary>Gets the number of items in this tree.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Gets an enumerator for the key-value pairs in this map.</summary>
        /// <returns>An enumerator for the key-value pairs in this tree.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return BSTTraversal.PreOrder(this.root).Select(n => n.ToKeyValuePair()).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
