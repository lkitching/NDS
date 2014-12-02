using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>Hash table implementation which uses closed addressing.</summary>
    /// <typeparam name="TKey">Key type for this table.</typeparam>
    /// <typeparam name="TValue">Value type for this table.</typeparam>
    public class ClosedAddressingHashTable<TKey, TValue> : IMap<TKey, TValue>
    {
        private int count;
        private SinglyLinkedList<KeyValuePair<TKey, TValue>>[] buckets;
        private readonly IEqualityComparer<TKey> keyComparer;
        private readonly double maxLoadFactor;

        /// <summary>Creates a new empty table using the default <see cref="IEqualityComparer{TKey}"/> for <typeparamref name="TKey"/>.</summary>
        public ClosedAddressingHashTable()
            : this(EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>Creates a new empty table using the given comparer for the keys.</summary>
        /// <param name="keyComparer"></param>
        public ClosedAddressingHashTable(IEqualityComparer<TKey> keyComparer)
            : this(keyComparer, 10)
        {
        }

        /// <summary>
        /// Creates a new empty table using the given key comparer and internal capacity. <paramref name="capacity"/> is used to choose the number of 
        /// internal buckets to hash keys into - the lowest power of 2 greater than the given capacity will be used as the initial number of buckets.
        /// </summary>
        /// <param name="keyComparer">Comparer for the keys in this table.</param>
        /// <param name="capacity">Suggested initial number of buckets.</param>
        public ClosedAddressingHashTable(IEqualityComparer<TKey> keyComparer, int capacity)
            : this(keyComparer, capacity, 2)
        {
        }

        /// <summary>
        /// Creates a new empty table using the given key comparer, initial capacity and maximum load factor. <paramref name="capacity"/> is used to choose the number of 
        /// internal buckets to hash keys into - the lowest power of 2 greater than the given capacity will be used as the initial number of buckets. <paramref name="maxLoadFactor"/> is
        /// the maximum load factor of the table before its internal capacity is increased. The load factor is the average number of items per bucket.
        /// </summary>
        /// <param name="keyComparer">Comparer for the keys in this table.</param>
        /// <param name="capacity">Suggested initial number of buckets.</param>
        /// <param name="maxLoadFactor">The maximum load factor for this table before the internal collection of buckets is resized.</param>
        public ClosedAddressingHashTable(IEqualityComparer<TKey> keyComparer, int capacity, double maxLoadFactor)
        {
            Contract.Requires(capacity > 0);
            Contract.Requires(maxLoadFactor > 0);

            this.keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            int bucketCount = GetInitialCapacity(capacity);
            this.buckets = new SinglyLinkedList<KeyValuePair<TKey,TValue>>[bucketCount];
            this.maxLoadFactor = maxLoadFactor;
        }

        /// <summary>Adds the associated key and value into this table, resizing if the new causes the maximum load factor to be exceeded.</summary>
        /// <see cref="IMap{TKey, TValue}.Add"/>
        public bool TryAdd(TKey key, TValue value)
        {
            Contract.Requires(key != null);
            int bucketIndex = this.GetBucketIndex(key);
            var list = this.buckets[bucketIndex];

            if (list != null && list.Any(GetMatcherForKey(key)))
            {
                //key already exists in table
                return false;
            }

            InsertIntoBucketAt(this.buckets, bucketIndex, key, value);
            this.count++;

            this.ResizeIfRequired();
            return true;
        }

        /// <summary>Associates the given key and value in this table, resizing if the new item causes the maximum load factor to be exceeded.</summary>
        /// <see cref="IMap{TKey, TValue}.Assoc(TKey, TValue)"/>
        public void Assoc(TKey key, TValue value)
        {
            Contract.Requires(key != null);

            int bucketIndex = this.GetBucketIndex(key);
            var list = this.buckets[bucketIndex];

            if (list == null)
            {
                var newList = InitList(key, value);
                this.buckets[bucketIndex] = newList;
                this.count++;
            }
            else
            {
                Debug.Assert(list.First != null);
                if (NodeMatchesKey(key, list.First))
                {
                    //replace list head with node containing new value
                    list.RemoveFirst();
                    list.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
                }
                else
                {
                    var pred = FindPredecessorNodeForKey(key, list);

                    if (pred == null)
                    {
                        //insert at front
                        list.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
                        this.count++;
                    }
                    else
                    {
                        //replace node
                        var toInsert = new SinglyLinkedListNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(key, value), pred.Next);
                        pred.UnlinkNext();
                        toInsert.InsertAfter(pred);
                    }
                }
            }

            this.ResizeIfRequired();
        }

        /// <see cref="IMap{TKey, TValue}.Delete(TKey)"/>
        public bool Delete(TKey key)
        {
            Contract.Requires(key != null);

            int bucketIndex = this.GetBucketIndex(key);
            var list = this.buckets[bucketIndex];

            if (list == null)
            {
                return false;
            }
            else
            {
                Debug.Assert(list.First != null);

                //if head of list matches then remove
                if (NodeMatchesKey(key, list.First))
                {
                    list.RemoveFirst();
                    this.buckets[bucketIndex] = null;
                    this.count--;
                    return true;
                }

                //search for match keeping track of the previous node
                var pred = FindPredecessorNodeForKey(key, list);

                if (pred != null)
                {
                    //found match
                    pred.UnlinkNext();
                    this.count--;
                    return true;
                }
                else return false;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Get(TKey)"/>
        public Maybe<TValue> Get(TKey key)
        {
            Contract.Requires(key != null);

            int bucketIndex = this.GetBucketIndex(key);
            var list = this.buckets[bucketIndex];

            if (list == null) return Maybe.None<TValue>();

            var node = list.FirstOrDefault(GetMatcherForKey(key));
            return node == null ? Maybe.None<TValue>() : Maybe.Some(node.Value.Value);
        }

        /// <summary>Clears this table.</summary>
        public void Clear()
        {
            Array.Clear(this.buckets, 0, this.buckets.Length);
            this.count = 0;
        }

        /// <summary>Gets the number of items in this table.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Gets the number of internal buckets in this table.</summary>
        public int Capacity
        {
            get { return this.buckets.Length; }
        }

        /// <summary>Gets the current load factor of this table.</summary>
        public double LoadFactor
        {
            get { return (double)this.count / this.buckets.Length; }
        }

        private int GetBucketIndex(TKey key)
        {
            Debug.Assert(this.buckets.Length > 0);
            return this.keyComparer.GetHashCode(key) % this.buckets.Length;
        }

        private Func<SinglyLinkedListNode<KeyValuePair<TKey, TValue>>, bool> GetMatcherForKey(TKey key)
        {
            int hc = this.keyComparer.GetHashCode(key);
            return n => this.keyComparer.GetHashCode(n.Value.Key) == hc && this.keyComparer.Equals(key, n.Value.Key);
        }

        private bool NodeMatchesKey(TKey key, SinglyLinkedListNode<KeyValuePair<TKey, TValue>> node)
        {
            return this.keyComparer.GetHashCode(key) == this.keyComparer.GetHashCode(node.Value.Key) &&
                this.keyComparer.Equals(key, node.Value.Key);
        }

        private void ResizeIfRequired()
        {
            if (this.LoadFactor > this.maxLoadFactor)
            {
                if (this.buckets.Length > 1 << 30)
                {
                    throw new InvalidOperationException("Cannot resize table - max capacity reached");
                }

                int nextCapacity = this.buckets.Length << 1;
                var newBuckets = new SinglyLinkedList<KeyValuePair<TKey, TValue>>[nextCapacity];

                //copy all elements into the new table
                foreach (var kvp in this)
                {
                    int newBucketIndex = this.keyComparer.GetHashCode(kvp.Key) % nextCapacity;
                    InsertIntoBucketAt(newBuckets, newBucketIndex, kvp.Key, kvp.Value);
                }

                this.buckets = newBuckets;
            }
        }

        private static void InsertIntoBucketAt(SinglyLinkedList<KeyValuePair<TKey, TValue>>[] buckets, int index, TKey key, TValue value)
        {
            Debug.Assert(index >= 0 && index <= buckets.Length);

            var list = buckets[index];
            if (list == null)
            {
                var newList = InitList(key, value);
                buckets[index] = newList;
            }
            else
            {
                 list.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        private SinglyLinkedListNode<KeyValuePair<TKey, TValue>> FindPredecessorNodeForKey(TKey key, SinglyLinkedList<KeyValuePair<TKey, TValue>> list)
        {
            Debug.Assert(list.First != null);
            Debug.Assert(!NodeMatchesKey(key, list.First));

            for (var pred = list.First; pred.Next != null; pred = pred.Next)
            {
                if (NodeMatchesKey(key, pred.Next)) return pred;
            }

            return null;
        }

        private static SinglyLinkedList<KeyValuePair<TKey, TValue>> InitList(TKey key, TValue value)
        {
            var newList = new SinglyLinkedList<KeyValuePair<TKey, TValue>>();
            newList.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
            return newList;
        }

        private static int GetInitialCapacity(int suggested)
        {
            if(suggested > 1 << 31)
            {
                return suggested;
            }

            int capacity = 1;
            while(suggested >= capacity)
            {
                Debug.Assert(capacity > 0, "Overflow while calculating initial capacity");
                capacity = capacity << 1;
            }

            return capacity;
        }

        /// <summary>Gets an enumerator for the key-value pairs in this table.</summary>
        /// <returns>An enumerator for the key-value pairs in this table.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < this.buckets.Length; ++i)
            {
                var bucket = this.buckets[i];
                if (bucket != null)
                {
                    foreach (var node in this.buckets[i])
                    {
                        yield return node.Value;
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
