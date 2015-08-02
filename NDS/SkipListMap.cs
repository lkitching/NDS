using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>Map implemented as a skip list.</summary>
    /// <typeparam name="TKey">The type of keys in this map.</typeparam>
    /// <typeparam name="TValue">The type of values in this map.</typeparam>
    public class SkipListMap<TKey, TValue> : IMap<TKey, TValue>
    {
        private const double DEFAULT_P = 0.5;

        private readonly IComparer<TKey> keyComparer;
        private readonly Random gen;
        private readonly double p;          //probability node with n neighbours also has n+1 neighbours
        private readonly int maxLevel;      //maximum node level in this list
        private int level = 1;              //current largest level in this list
        private int count = 0;

        private readonly SkipListNode<TKey, TValue> sentinal;

        /// <summary>Creates a new empty map with the default key comparer.</summary>
        public SkipListMap() : this(Comparer<TKey>.Default, DEFAULT_P, new Random()) { }

        /// <summary>Creates a new empty map with the given key comparer.</summary>
        /// <param name="keyComparer">Used to compare keys in this map.</param>
        public SkipListMap(IComparer<TKey> keyComparer) : this(keyComparer, DEFAULT_P, new Random()) { }

        /// <summary>
        /// Creates a new empty map with the given key comparer, random generator and proportion of nodes
        /// of level n to n+1. New nodes are given n links to nodes later in the list - the number of links
        /// depends on the value of <paramref name="p"/> and the random generator <paramref name="randomGen"/>.
        /// </summary>
        /// <param name="keyComparer">Comparer for the keys in the list.</param>
        /// <param name="p">The proportion of new nodes of level (n+1) to those with level n.</param>
        /// <param name="randomGen">Random number generator used to generate the level of new nodes.</param>
        public SkipListMap(IComparer<TKey> keyComparer, double p, Random randomGen)
        {
            Contract.Requires(keyComparer != null);
            Contract.Requires(p > 0.0 && p <= 1);
            Contract.Requires(randomGen != null);

            this.keyComparer = keyComparer;
            this.p = p;

            this.maxLevel = (int)Math.Ceiling(Math.Log(int.MaxValue, 1.0 / p));
            Debug.Assert(maxLevel > 1, "Need at least one level");
            
            this.gen = randomGen;
            this.sentinal = new SkipListNode<TKey, TValue>(default(TKey), default(TValue), this.maxLevel);
        }

        /// <see cref="IMap{TKey, TValue}.Get"/>
        public Maybe<TValue> Get(TKey key)
        {
            var search = SearchFor(key);
            return search.Found ? Maybe.Some(search.MatchingNode.Value) : Maybe.None<TValue>();
        }

        /// <see cref="IMap{TKey, TValue}.TryAdd"/>
        public bool TryAdd(TKey key, TValue value)
        {
            var search = SearchFor(key);
            if (search.Found) return false;
            else
            {
                ApplyInsert(key, value, search);
                return true;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            var search = SearchFor(key);
            if (search.Found)
            {
                Debug.Assert(search.MatchingNode != null, "matching node should exist");
                search.MatchingNode.Value = value;
            }
            else
            {
                ApplyInsert(key, value, search);
            }
        }

        /// <see cref="IMap{TKey, TValue}.Delete"/>
        public bool Delete(TKey key)
        {
            var search = SearchFor(key);
            if (search.Found)
            {
                var removing = search.MatchingNode;
                Debug.Assert(removing != null, "matching node should exist");

                //unlink node
                for (int i = 0; i < removing.Level; ++i)
                {
                    search.LevelPredecessors[i] = removing[i];
                }

                this.count--;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ApplyInsert(TKey key, TValue value, SearchResult search)
        {
            Debug.Assert(!search.Found, "No insert required for successful search");

            var nodeLevel = RandomNewNodeLevel();
            var newNode = new SkipListNode<TKey, TValue>(key, value, nodeLevel);

            //link in node
            //first link all predecessors up to the current level of the list
            //it is possible the new node has a level greater than the level of the list - in that
            //case its predecessor at all higher levels is the sentinal node
            for (int i = 0; i < Math.Min(nodeLevel, this.level); ++i)
            {
                newNode[i] = search.LevelPredecessors[i][i];
                search.LevelPredecessors[i][i] = newNode;
            }

            if (nodeLevel > this.level)
            {
                for (int i = this.level; i < nodeLevel; ++i)
                {
                    newNode[i] = this.sentinal[i];
                    this.sentinal[i] = newNode;
                }

                //update list level
                this.level = nodeLevel;
            }

            this.count++;
        }

        private int RandomNewNodeLevel()
        {
            int level = 1;
            while (this.gen.NextDouble() < this.p && level < this.maxLevel)
            {
                level++;
            }

            return level;
        }

        /// <summary>Clears this map.</summary>
        public void Clear()
        {
            for (int i = this.maxLevel - 1; i >= 0; --i)
            {
                this.sentinal[i] = null;
            }
            this.level = 1;
            this.count = 0;
        }

        /// <summary>Gets the number of elements in this map.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Returns an enumerator for the key-value pairs in this map.</summary>
        /// <returns>
        /// An enumerator for the pairs in this map. The keys are returned in sorted order according
        /// to the key comparer given on construction.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var head = this.sentinal.Next;
            return head.EnumerateFrom().Select(n => new KeyValuePair<TKey, TValue>(n.Key, n.Value)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private SearchResult SearchFor(TKey key)
        {
            var predecessors = new SkipListNode<TKey, TValue>[this.level];
            for(int i = 0; i < this.level; ++i)
            {
                predecessors[i] = this.sentinal;
            }

            if (this.Count == 0)
            {
                return new SearchResult(false, null, predecessors);
            }

            var currentNode = this.sentinal;

            for (int i = this.level - 1; i >= 0; --i)
            {
                var next = currentNode[i];
                while (next != null)
                {
                    int c = this.keyComparer.Compare(key, next.Key);
                    if (c > 0)
                    {
                        //keep searching at this level
                        currentNode = next;
                        next = currentNode[i];
                    }
                    else
                    {
                        //finished search at this level so move to next level down
                        break;
                    }
                }

                //record predecessor for this level
                predecessors[i] = currentNode;
            }

            //if the key exists in this list, it is the successor of the current node
            Debug.Assert(currentNode != null, "predecessor should exist");

            var candidate = currentNode.Next;
            bool found = candidate != null && this.keyComparer.Compare(key, candidate.Key) == 0;
            return new SearchResult(found, found ? candidate : null, predecessors);
        }

        private struct SearchResult
        {
            public SearchResult(bool found, SkipListNode<TKey, TValue> matchingNode, SkipListNode<TKey, TValue>[] levelPredecessors)
                : this()
            {
                this.Found = found;
                this.MatchingNode = matchingNode;
                this.LevelPredecessors = levelPredecessors;
            }

            public bool Found { get; private set; }
            public SkipListNode<TKey, TValue> MatchingNode { get; private set; }
            public SkipListNode<TKey, TValue>[] LevelPredecessors { get; private set; }
        }
    }
}
