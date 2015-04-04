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
            var pendingInsert = FindInsertFor(key, value);
            if (pendingInsert.Item1 == InsertType.SetValue)
            {
                //key already exists so don't update
                return false;
            }
            else
            {
                ApplyInsert(pendingInsert.Item1, key, value, pendingInsert.Item2);
                return true;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            var pendingInsert = FindInsertFor(key, value);
            ApplyInsert(pendingInsert.Item1, key, value, pendingInsert.Item2);
        }

        /// <summary>Applies an update to associate a key and value in this tree.</summary>
        /// <param name="type">The type of update to perform.</param>
        /// <param name="key">The key to associate in this tree.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <param name="parent">
        /// The affected node, or null if this tree is empty. In that case <paramref name="type"/>
        /// should be Root.
        /// </param>
        private void ApplyInsert(InsertType type, TKey key, TValue value, BSTNode<TKey, TValue> parent)
        {
            switch (type)
            {
                case InsertType.SetValue:
                    parent.Value = value;
                    //return immediately and don't update count
                    return;

                case InsertType.Root:
                    Debug.Assert(parent == null);
                    this.root = BSTNode.Create(key, value);
                    break;

                case InsertType.LeftChild:
                    Debug.Assert(parent.Left == null);
                    parent.Left = BSTNode.Create(key, value);
                    break;

                case InsertType.RightChild:
                    Debug.Assert(parent.Right == null);
                    parent.Right = BSTNode.Create(key, value);
                    break;
            }

            this.count++;
        }

        /// <summary>Calculates how to perform an update to associated the given key and value in this tree.</summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>A pair containing the affected node and the type of modification to apply to it.</returns>
        private Tuple<InsertType, BSTNode<TKey, TValue>> FindInsertFor(TKey key, TValue value)
        {
            var current = this.root;

            while (current != null)
            {
                var c = this.comp.Compare(key, current.Key);
                if (c == 0)
                {
                    //key already exists so update value of current node
                    return Tuple.Create(InsertType.SetValue, current);
                }
                else if (c < 0)
                {
                    //insert in left subtree
                    if (current.Left == null)
                    {
                        return Tuple.Create(InsertType.LeftChild, current);
                    }
                    else { current = current.Left; }
                }
                else
                {
                    //insert in right subtree
                    if (current.Right == null)
                    {
                        return Tuple.Create(InsertType.RightChild, current);
                    }
                    else { current = current.Right; }
                }
            }

            //should only get here if tree is empty
            Debug.Assert(current == null);
            Debug.Assert(this.count == 0);
            return Tuple.Create(InsertType.Root, current);
        }

        private enum InsertType
        {
            Root,
            LeftChild,
            RightChild,
            SetValue
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
