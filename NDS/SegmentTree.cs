using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Segment tree containing summaries of sub-ranges within a given collection.</summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class SegmentTree<T>
    {
        private readonly IReadOnlyList<T> col;
        private readonly T[] heap;
        private readonly Func<T, T, T> summarise;

        /// <summary>Constructs a segment tree for the given collection and summary function.</summary>
        /// <param name="col"></param>
        /// <param name="summarise">Combines summary values for two sub-ranges into one for the combined sub-range.</param>
        public SegmentTree(IReadOnlyList<T> col, Func<T, T, T> summarise)
        {
            Contract.Requires(col != null);
            Contract.Requires(summarise != null);

            this.col = col;
            this.summarise = summarise;

            int heapSize = (int)(2 * Math.Pow(2.0, Math.Floor(Math.Log(col.Count, 2.0)) + 1));
            this.heap = new T[heapSize];

            if(col.Count > 0)
            {
                this.BuildHeap(1, 0, this.col.Count - 1);
            }
        }

        private void BuildHeap(int heapIndex, int startIndex, int endIndex)
        {
            if(startIndex == endIndex)
            {
                //leaf node
                this.heap[heapIndex] = this.col[startIndex];
            }
            else
            {
                //calculate left and right children
                int leftChildIndex = heapIndex * 2;
                int rightChildIndex = leftChildIndex + 1;

                //split range for each child
                int midIndex = (startIndex + endIndex) / 2;
                this.BuildHeap(leftChildIndex, startIndex, midIndex);
                this.BuildHeap(rightChildIndex, midIndex + 1, endIndex);

                //combine left and right values into value for current node
                this.heap[heapIndex] = this.summarise(this.heap[leftChildIndex], this.heap[rightChildIndex]);
            }
        }

        /// <summary>Calculates the summary value for a given range over the source collection.</summary>
        /// <param name="range">The range to query.</param>
        /// <returns>Summary for the subrange <paramref name="range"/> over the source collection.</returns>
        /// <exception cref="ArgumentException">If <paramref name="range"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="range"/> is not contained within the range [0, count) for the source collection.
        /// </exception> 
        public T FindRange(IntRange range)
        {
            if(range.IsEmpty)
            {
                throw new ArgumentException("Search range must be non-empty");
            }

            IntRange colRange = new IntRange(0, this.col.Count);
            if(! range.IsContainedWithin(colRange))
            {
                string message = string.Format("Search range must be contained within the range {0}", colRange);
                throw new ArgumentOutOfRangeException(message);
            }
            else
            {
                var result = Search(1, 0, this.col.Count - 1, range.Start, range.End - 1);
                return result.Value;
            }
        }

        /// <summary>
        /// Updates all summaries containing the item at the given index in the source collection.
        /// This should be called if an item in the source collection is modified.
        /// </summary>
        /// <param name="index">The index of the updated element in the source collection.</param>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is out of range for the source collection.</exception>
        public void Updated(int index)
        {
            if(index < 0 || index >= this.col.Count)
            {
                string message = string.Format("Updated index must be in range [0, {0})", this.col.Count);
                throw new IndexOutOfRangeException(message);
            }
            Update(1, 0, this.col.Count - 1, index);
        }

        private void Update(int heapIndex, int low, int hi, int targetIndex)
        {
            if(low == hi)
            {
                Debug.Assert(low == targetIndex, "Expected range to be [targetIndex, targetIndex]");
                this.heap[heapIndex] = this.col[targetIndex];
            }
            else
            {
                //split range into [low, mid] and (mid, hi]
                //check which range the updated index is within
                //update child node for range containing the updated index
                int midIndex = (low + hi) / 2;
                int leftChildIndex = heapIndex * 2;
                int rightChildIndex = leftChildIndex + 1;

                if(targetIndex <= midIndex)
                {
                    Update(leftChildIndex, low, midIndex, targetIndex);
                }
                else
                {
                    Update(rightChildIndex, midIndex + 1, hi, targetIndex);
                }

                //re-calculate this node
                this.heap[heapIndex] = this.summarise(this.heap[leftChildIndex], this.heap[rightChildIndex]);
            }
        }

        private Maybe<T> Search(int heapIndex, int low, int hi, int searchBegin, int searchEnd)
        {
            if(searchBegin > hi || searchEnd < low)
            {
                //search interval does not overlap current interval
                return Maybe.None<T>();
            }

            if(low >= searchBegin && hi <= searchEnd)
            {
                //current interval is within search interval
                return Maybe.Some(this.heap[heapIndex]);
            }

            //split current search range into two and find index for each half
            int midIndex = (low + hi) / 2;
            int leftHeapIndex = heapIndex * 2;
            int rightHeapIndex = leftHeapIndex + 1;

            var leftValue = Search(leftHeapIndex, low, midIndex, searchBegin, searchEnd);
            var rightValue = Search(rightHeapIndex, midIndex + 1, hi, searchBegin, searchEnd);

            //if either sub-interval does not overlap the search range, return the other
            //(since it must contain the entire search range)
            if (!leftValue.HasValue) return rightValue;
            else if (!rightValue.HasValue) return leftValue;
            else
            {
                //combine values for left and right sub-ranges
                return Maybe.Some(this.summarise(leftValue.Value, rightValue.Value));
            }
        }
    }
}
