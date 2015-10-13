using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Unbalanced binary search tree.</summary>
    /// <typeparam name="TKey">Type of keys in this tree.</typeparam>
    /// <typeparam name="TValue">Type of values in this key.</typeparam>
    public class BinarySearchTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IComparer<TKey> comp;
        private int count = 0;
        private BSTNode<TKey, TValue> root = null;

        /// <summary>Creates an empty tree with the default comparer for keys.</summary>
        public BinarySearchTree()
            : this(Comparer<TKey>.Default)
        {
        }

        /// <summary>Creates an empty tree with the given kek comparer.</summary>
        /// <param name="comparer">The comparer to use for keys.</param>
        public BinarySearchTree(IComparer<TKey> comparer)
        {
            Contract.Requires(comparer != null);
            this.comp = comparer;
        }

        /// <summary>Finds the value mapped to the given key in this tree if any exists.</summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The value mapped to the given key if it exists in this tree otherwise None.</returns>
        public Maybe<TValue> Get(TKey key)
        {
            return BSTNode.Get(this.root, key, this.comp);
        }

        /// <see cref="IMap{TKey, TValue}.TryAdd"/>
        public bool TryAdd(TKey key, TValue value)
        {
            var context = BSTSearch.SearchFor<BSTNode<TKey, TValue>, TKey, TValue>(this.root, key, this.comp);
            if (context.Found)
            {
                //key exists so don't insert
                return false;
            }
            else
            {
                ApplyInsert(context, key, value);
                return true;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            var context = BSTSearch.SearchFor<BSTNode<TKey, TValue>, TKey, TValue>(this.root, key, this.comp);
            if (context.Found)
            {
                context.MatchingNode.Value = value;
            }
            else
            {
                ApplyInsert(context, key, value);
            }
        }

        /// <summary>Applies an update to associate a key and value in this tree.</summary>
        /// <param name="type">The type of update to perform.</param>
        /// <param name="key">The key to associate in this tree.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <param name="parent">
        /// The affected node, or null if this tree is empty. In that case <paramref name="type"/>
        /// should be Root.
        /// </param>
        private void ApplyInsert(IBSTSearchContext<BSTNode<TKey, TValue>> context, TKey key, TValue value)
        {
            var newNode = new BSTNode<TKey, TValue>(key, value);
            if (context.SearchPath.Count == 0)
            {
                Debug.Assert(this.count == 0, "Tree should be empty");
                Debug.Assert(this.root == null, "Empty tree should have null root");
                this.root = newNode;
            }
            else
            {
                var parentBranch = context.SearchPath[context.SearchPath.Count - 1];
                var parent = parentBranch.Node;
                if (parentBranch.Direction == BranchDirection.Left)
                {
                    parent.Left = newNode;
                }
                else
                {
                    parent.Right = newNode;
                }
            }

            this.count++;
        }

        /// <summary>Deletes the given key from this tree.</summary>
        /// <param name="key">The key to delete.</param>
        /// <returns>Whether the key existed in this tree before the delete operaiton.</returns>
        public bool Delete(TKey key)
        {
            var result = BSTNode.Delete(this.root, key, this.comp);
            this.root = result.Item2;
            bool deleted = result.Item1;

            if (deleted) { this.count--; }

            return deleted;
        }

        /// <summary>Deletes all items from this tree.</summary>
        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        /// <summary>Gets the number of elements in this map.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Performs an in-order traversal of the pairs in this tree.</summary>
        /// <returns>An enumerator for the in-order traversal of this tree.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return BSTTraversal.InOrder(this.root).Select(n => n.ToKeyValuePair()).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
