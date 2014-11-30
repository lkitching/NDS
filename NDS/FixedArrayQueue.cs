using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS 
{
    /// <summary>Queue implementation with a fixed capacity.</summary>
    /// <typeparam name="T">The type of items in the queue.</typeparam>
    public class FixedArrayQueue<T> : IQueue<T>
    {
        private T[] items;
        private readonly int capacity;
        private int headIndex;
        private int tailIndex;

        /// <summary>Creates a new instance of this class with the given capacity.</summary>
        /// <param name="capacity">The capacity for this queue.</param>
        public FixedArrayQueue(int capacity)
        {
            Contract.Requires(capacity > 0);
            this.capacity = capacity;
            this.items = new T[this.capacity + 1];

            this.headIndex = this.items.Length;
            this.tailIndex = 0;
        }

        /// <summary>Enqueues an item at the end of this queue if there is sufficient capacity.</summary>
        /// <param name="item">The item to enqueue.</param>
        /// <exception cref="InvalidOperationException">If this queue is full.</exception>
        public void Enqueue(T item)
        {
            if (this.IsFull) throw new InvalidOperationException("Cannot insert item - queue is full");

            //insert new item and move the tail index to the next available space, wrapping around if necessary
            this.items[this.tailIndex] = item;
            this.tailIndex = (this.tailIndex + 1) % this.items.Length;

            //NOTE: The tail index should never occupy the 'hole' at the end of the item array
            Debug.Assert(this.tailIndex != this.items.Length);
        }

        /// <see cref="IQueue{T}.Dequeue"/>
        public T Dequeue()
        {
            if (this.Count == 0) throw new InvalidOperationException("Cannot dequeue item - queue is empty");

            //move head over item to dequeue if it in the 'hole' beyond the end of the queue array
            this.headIndex = this.headIndex % this.items.Length;
            T removed = this.items[this.headIndex];

            //move head to next item
            this.headIndex++;

            return removed;
        }

        /// <summary>Returns true if this queue is not full.</summary>
        public bool CanEnqueue
        {
            get { return !this.IsFull; }
        }

        /// <summary>Whether this queue is full.</summary>
        public bool IsFull
        {
            get
            {
                int nextTailIndex = (this.tailIndex + 1) % this.items.Length;
                int effectiveHeadIndex = this.headIndex % this.items.Length;

                //this queue is full if the next head index is equal to the effective head index
                return nextTailIndex == effectiveHeadIndex;
            }
        }

        /// <summary>Gets the number of items in this queue.</summary>
        public int Count
        {
            get
            {
                //calculate the effective index of the head - note this should never be greater than the current tail index
                int effectiveHeadIndex = this.headIndex % this.items.Length;
                Debug.Assert(effectiveHeadIndex <= this.tailIndex);

                return this.tailIndex - effectiveHeadIndex;
            }
        }

        /// <summary>Gets the capacity of this queue.</summary>
        public int Capacity
        {
            get { return this.capacity; }
        }

        /// <summary>Gets an enumerator for the items in this queue.</summary>
        /// <returns>An enumerator for this collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = this.headIndex % this.items.Length; i != this.tailIndex; i = (i + 1) % this.items.Length)
            {
                yield return this.items[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
