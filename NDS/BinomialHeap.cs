using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Priority queue represented by a forest of left-ordered heaps.</summary>
    /// <typeparam name="T">The type of items in this queue.</typeparam>
    public class BinomialHeap<T> : IPriorityQueue<T>
    {
        private const int NUM_FORESTS = 31;
        private readonly BinomialTreeNode<T>[] trees = new BinomialTreeNode<T>[NUM_FORESTS];
        private readonly IComparer<T> comparer;
        private int count = 0;

        /// <summary>Creates a new instance of this class with the default comparison for the element type.</summary>
        public BinomialHeap() : this(Comparer<T>.Default) { }

        /// <summary>Creates a new instance of this class with the given comparison method for queue elements.</summary>
        /// <param name="comparer">The ordering to use for items in the queue.</param>
        public BinomialHeap(IComparer<T> comparer)
        {
            Contract.Requires(comparer != null);
            this.comparer = comparer;
        }

        /// <see cref="IPriorityQueue{T}.GetMinimum"/>
        public T GetMinimum()
        {
            this.GuardNotEmpty();

            int minTreeIndex = GetMinTreeIndex();
            T min = this.trees[minTreeIndex].Value;
            return min;
        }

        private int GetMinTreeIndex()
        {
            Debug.Assert(this.count > 0, "No minimum in empty heap");

            //find index of first non-null tree in the heap (must exist)
            int minTreeIndex = -1;
            for (int i = 0; i < this.trees.Length; ++i)
            {
                if (trees[i] != null)
                {
                    minTreeIndex = i;
                    break;
                }
            }

            Debug.Assert(minTreeIndex >= 0, "No trees found in non-empty heap");

            T currentMin = trees[minTreeIndex].Value;
            for (int i = minTreeIndex + 1; i < this.trees.Length; ++i)
            {
                if (this.trees[i] != null)
                {
                    T val = this.trees[i].Value;
                    if (this.comparer.Compare(val, currentMin) < 0)
                    {
                        currentMin = val;
                        minTreeIndex = i;
                    }
                }
            }

            return minTreeIndex;
        }

        /// <see cref="IPriorityQueue{T}.RemoveMinimum"/>
        public T RemoveMinimum()
        {
            this.GuardNotEmpty();

            int minTreeIndex = this.GetMinTreeIndex();
            var tree = this.trees[minTreeIndex];

            this.trees[minTreeIndex] = null;

            if (minTreeIndex > 0)
            {
                //the tree at index i contains 2^i nodes. Since the root is being removed, it will contain
                //2^i - 1 nodes. The tree comprises i left-heaps of size 1,2,..2^(i-1). These heaps should be
                //extracted from the removing tree and then merged back into this heap.
                var tempForest = new BinomialTreeNode<T>[NUM_FORESTS];
                var current = tree.Left;

                for (int i = minTreeIndex - 1; i >= 0; --i)
                {
                    Debug.Assert(current != null);

                    //set the current root in position in the 'deconstructed' forrest. Once the root
                    //has been set and moved to the next child (the right child of the current node)
                    //its right subtree must be removed to maintain the left-heap property
                    tempForest[i] = current;

                    var tmp = current;

                    //move to next node and clear right subtree of current node to maintain
                    //left heap property
                    current = current.Right;
                    tmp.Right = null;
                }

                MergeForests(this.trees, tempForest);
            }

            this.count--;
            return tree.Value;
        }

        /// <see cref="IPriorityQueue{T}.Insert"/>
        public void Insert(T value)
        {
            if (this.count == int.MaxValue)
            {
                throw new InvalidOperationException("Heap full");
            }

            var carry = new BinomialTreeNode<T>(value);
            for (int i = 0; i < this.trees.Length; ++i)
            {
                Debug.Assert(carry != null, "Carry tree null");
                if (this.trees[i] == null)
                {
                    this.trees[i] = carry;
                    break;
                }
                else
                {
                    //merge the tree at the current position with the previously-carried tree
                    carry = JoinTrees(carry, this.trees[i]);
                    this.trees[i] = null;
                }
            }

            this.count++;
        }

        /// <summary>Removes all the elements in this queue.</summary>
        public void Clear()
        {
            Array.Clear(this.trees, 0, this.trees.Length);
            this.count = 0;
        }

        /// <summary>Gets the number of items in this queue.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>
        /// Merges all the items from the given binomial heap into this heap. After this operation all the elements in
        /// <paramref name="other"/> will have been added to this queue. In addition <paramref name="other"/> will no 
        /// longer be useable and should not be accessed.
        /// </summary>
        /// <param name="other">The binomial heap to merge into this one.</param>
        public void Merge(BinomialHeap<T> other)
        {
            long mergedCount = (long)this.count + (long)other.count;
            if (mergedCount > int.MaxValue) throw new InvalidOperationException("Merged heap would exceed maximum size");

            MergeForests(this.trees, other.trees);
            this.count = (int)mergedCount;
        }

        private void GuardNotEmpty()
        {
            if (this.count == 0) throw new InvalidOperationException("Queue is empty");
        }

        private BinomialTreeNode<T> JoinTrees(BinomialTreeNode<T> n1, BinomialTreeNode<T> n2)
        {
            BinomialTreeNode<T> smaller, larger;

            if (this.comparer.Compare(n1.Value, n2.Value) <= 0)
            {
                smaller = n1;
                larger = n2;
            }
            else
            {
                smaller = n2;
                larger = n1;
            }

            //larger node adopts left subtree of smaller node and makes it it's right subtree
            //(which should be null). Larger node then becomes the left subtree of the smaller node
            //to maintain heap order
            Debug.Assert(larger.Right == null, "Heap violates left property");

            larger.Right = smaller.Left;
            smaller.Left = larger;
            return smaller;
        }

        private void MergeForests(BinomialTreeNode<T>[] dest, BinomialTreeNode<T>[] source)
        {
            Debug.Assert(dest.Length == source.Length, "Forests should be same length");

            BinomialTreeNode<T> carry = null;

            for (int i = 0; i < dest.Length; ++i)
            {
                var destTree = dest[i];
                var sourceTree = source[i];

                if (carry == null)
                {
                    //no carry
                    if (destTree == null)
                    {
                        dest[i] = sourceTree;
                    }
                    else
                    {
                        if (sourceTree != null)
                        {
                            carry = JoinTrees(sourceTree, destTree);
                            dest[i] = null;
                        }
                    }
                }
                else
                {
                    //carry
                    if (sourceTree == null)
                    {
                        if (destTree == null)
                        {
                            dest[i] = carry;
                            carry = null;
                        }
                        else
                        {
                            carry = JoinTrees(carry, destTree);
                            dest[i] = null;
                        }
                    }
                    else
                    {
                        //source and carry both non-null so carry will be propagated. destination tree will remain unchaged in
                        //both cases (destTree null or non-null)
                        carry = JoinTrees(carry, sourceTree);
                    }
                }
            }
        }

        private class BinomialTreeNode<V> : IBinaryNode<BinomialTreeNode<V>>
        {
            public BinomialTreeNode(V value)
            {
                this.Value = value;
            }

            public V Value { get; private set; }

            public BinomialTreeNode<V> Left { get; set; }

            public BinomialTreeNode<V> Right { get; set; }
        }
    }
}
