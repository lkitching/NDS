using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NDS 
{
    public class BinaryHeap<T>
    {
        private readonly IComparer<T> comparer;
        private T[] items;
        private int maxDepth;
        private int count = 0;

        public BinaryHeap()
            : this(Comparer<T>.Default)
        {
        }

        public BinaryHeap(IComparer<T> comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;

            this.maxDepth = 4;
            int initialCapacity = GetCapacityForDepth(this.maxDepth);
            this.items = new T[initialCapacity];
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

        public T RemoveMin()
        {
            this.GuardNotEmpty();

            //get min
            T min = this.items[0];

            //move the 'last' item in the tree into the root position
            this.items[0] = this.items[this.count - 1];
            this.items[this.count - 1] = default(T);        //remove extra reference to item
            
            //fix heap
            this.FixDown(0);

            this.count--;
            return min;
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

        private void FixUp(int nodeIndex)
        {
            while (nodeIndex > 0)
            {
                int parentIndex = GetParentIndex(nodeIndex);
                
                //swap current node and parent if current < parent
                if (this.comparer.Compare(this.items[nodeIndex], this.items[parentIndex]) < 0)
                {
                    this.SwapNodes(nodeIndex, parentIndex);
                    nodeIndex = parentIndex;
                }
                else break;
            }
        }

        private void FixDown(int nodeIndex)
        {
            int? minChildIndex = this.GetMinChildIndex(nodeIndex);

            //swap node with min child until it is no smaller
            while (minChildIndex.HasValue && this.comparer.Compare(this.items[nodeIndex], this.items[minChildIndex.Value]) > 0)
            {
                this.SwapNodes(nodeIndex, minChildIndex.Value);
                minChildIndex = this.GetMinChildIndex(minChildIndex.Value);
            }
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

        private int? GetMinChildIndex(int parentIndex)
        {
            Debug.Assert(parentIndex >= 0);

            //left child is at 2n, right at 2n+1
            int leftChildIndex = parentIndex * 2;
            int rightChildIndex = leftChildIndex + 1;

            if (leftChildIndex >= this.count) return null;                      //no children
            else if (rightChildIndex >= this.count) return leftChildIndex;      //left child only
            else
            {
                //both children exist so find smallest
                //NOTE: right child is chosen if both are equal
                return this.comparer.Compare(this.items[leftChildIndex], this.items[rightChildIndex]) < 0
                    ? leftChildIndex
                    : rightChildIndex;
            }
        }

        private void SwapNodes(int i, int j)
        {
            T temp = this.items[i];
            this.items[i] = this.items[j];
            this.items[j] = temp;
        }

        private static int GetCapacityForDepth(int depth)
        {
            Debug.Assert(depth > 0);

            if (depth > 31) throw new ArgumentOutOfRangeException("Cannot create tree with depth > 31");
            return (1 << depth) - 1;
        }

        private static int GetParentIndex(int childIndex)
        {
            if(childIndex == 0) return -1;
            return ((childIndex + 1) / 2) - 1;
        }
    }
}
