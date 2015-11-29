using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NDS 
{
    public class BinaryHeap<T> : IPriorityQueue<T>
    {
        private readonly IComparer<T> comparer;
        private T[] items;
        private int maxDepth;
        private int count;

        public BinaryHeap()
            : this(Comparer<T>.Default)
        {
        }

        public BinaryHeap(IComparer<T> comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;
            this.InitialiseEmpty();
        }

        private void InitialiseEmpty()
        {
            this.maxDepth = 4;
            int initialCapacity = GetCapacityForDepth(this.maxDepth);
            this.items = new T[initialCapacity];
            this.count = 0;
        }

        public void Insert(T value)
        {
            //ensure space for new item
            this.EnsureCapacityForInsert();

            //insert item at the 'end' of the tree (i.e. right-most item at the deepest level)
            this.items[this.count] = value;

            //fix heap property
            this.FixUp(this.count);
            this.count++;
        }

        public T RemoveMinimum()
        {
            this.GuardNotEmpty();

            //get min
            T min = this.items[0];

            //move the 'last' item in the tree into the root position
            this.items[0] = this.items[this.count - 1];
            this.items[this.count - 1] = default(T);        //remove extra reference to item
            
            //fix heap
            this.count--;
            this.FixDown(0);

            return min;
        }

        public T GetMinimum()
        {
            return Min;
        }

        public T Min
        {
            get
            {
                this.GuardNotEmpty();

                Debug.Assert(this.items.Length > 0);
                return this.items[0];
            }
        }

        public int Count
        {
            get { return this.count; }
        }

        public void Clear()
        {
            this.InitialiseEmpty();
        }

        private void FixUp(int nodeIndex)
        {
            NDS.Algorithms.BinaryHeapOperations.FixUp(this.items, nodeIndex, 0, this.comparer);
        }

        private void FixDown(int nodeIndex)
        {
            NDS.Algorithms.BinaryHeapOperations.FixDown(this.items, nodeIndex, 0, this.count - 1, this.comparer);
        }

        private void EnsureCapacityForInsert()
        {
            if (this.count < this.items.Length - 1) return;

            int newCapacity = GetCapacityForDepth(this.maxDepth + 1);
            Debug.Assert(newCapacity > this.items.Length);

            T[] newItems = new T[newCapacity];
            Array.Copy(this.items, newItems, this.items.Length);
            this.items = newItems;

            this.maxDepth++;
        }

        private void GuardNotEmpty()
        {
            if (this.count < 1) throw new InvalidOperationException("Heap is empty");
        }

        private static int GetCapacityForDepth(int depth)
        {
            Debug.Assert(depth > 0);

            if (depth > 31) throw new ArgumentOutOfRangeException("Cannot create tree with depth > 31");
            return (1 << depth) - 1;
        }
    }
}
