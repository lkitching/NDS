using System.Collections.Generic;
using System.Diagnostics;

namespace NDS.Algorithms.Sorting
{
    /// <summary>Represents the result of partitioning a collection into three around a pivot.</summary>
    public struct ThreeWayPartitionResult
    {
        public ThreeWayPartitionResult(int eqStartIndex, int gtStartIndex) : this()
        {
            this.EqStartIndex = eqStartIndex;
            this.GtStartIndex = gtStartIndex;
        }

        public int EqStartIndex { get; private set; }
        public int GtStartIndex { get; private set; }
    }

    public static class ThreeWayPartition
    {
        

        /// <summary>
        /// Chooses a pivot element in the range [fromIndex, toIndex) of an input array and partitions it into three: items less than, equal to 
        /// and greater than the pivot. The return value indicates the start and end of the equal partition within the partition range. This should
        /// always be non-empty when the input range is non-empty since it contains at least the pivot element. Elements within the range
        /// [fromIndex, result.EqStartIndex) are less than the pivot
        /// [result.EqStartIndex, result.GtStartIndex) are equal to the pivot
        /// [result.GtStartIndex, toIndex) are greater than the pivot
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="items">The collection to partition.</param>
        /// <param name="fromIndex">The start index of the range to partition within <paramref name="items"/>.</param>
        /// <param name="toIndex">The exclusive end index of the rage to partition.</param>
        /// <param name="comp">Comparer for the elements in the input.</param>
        /// <returns>
        /// A <see cref="ThreeWayPartitionResult"/> which indicates the range of the three partitions within <paramref name="items"/>.
        /// </returns>
        public static ThreeWayPartitionResult Partition<T>(IList<T> items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            if (IntRange.RangeCount(fromIndex, toIndex) <= 1) return new ThreeWayPartitionResult(fromIndex, toIndex);

            int pivotIndex = Median.EstimateMedianIndex(items, fromIndex, toIndex, comp);
            T pivot = items[pivotIndex];

            //move pivot into left-hand equal partition
            items.SwapIndexed(pivotIndex, fromIndex);

            //pointer to left and right end of equal partitions
            int nextEqLeftIdx = fromIndex + 1;
            int nextEqRightIdx = toIndex - 1;

            //left and right 'sweep' indexes
            int l = fromIndex + 1;
            int r = toIndex - 1;

            while (true)
            {
                //sweep left-to-right through array until an item > pivot is found
                //if an item is = to pivot, move it to the left-hand equal partition
                while (l <= r && comp.Compare(items[l], pivot) < 1)
                {
                    if (comp.Compare(items[l], pivot) == 0)
                    {
                        items.SwapIndexed(nextEqLeftIdx, l);
                        nextEqLeftIdx++;
                    }

                    l++;
                }

                //sweep right-to-left through array until an item < pivot is found
                //if an item is = to pivot, move it to the right-hand equal partition
                while (r >= l && comp.Compare(items[r], pivot) >= 0)
                {
                    if (comp.Compare(items[r], pivot) == 0)
                    {
                        items.SwapIndexed(nextEqRightIdx, r);
                        nextEqRightIdx--;
                    }

                    r--;
                }

                //done if sweep pointers have crossed
                if (l > r) break;

                //l points to item > pivot and item to one < pivot
                //swap items and advance both sweep pointers
                items.SwapIndexed(l, r);
                l++;
                r--;
            }

            //sweep completes so range should be split into 4 partitions:
            Debug.Assert(r == l - 1);

            //[fromIndex, nextEqLeftIndex) = pivot
            //[nextEqLeftIndex, r] < pivot
            //[l, nextEqRightIndex] > pivot
            //(nextEqRightIndex, toIndex) = pivot

            //move items from two end partitions into the middle

            //move left equal partition
            int leftEqCount = nextEqLeftIdx - fromIndex;
            for (int i = 0; i < leftEqCount; ++i)
            {
                items.SwapIndexed(fromIndex + i, r - i);
            }

            //move right equal partition
            int rightEqCount = toIndex - 1 - nextEqRightIdx;
            for (int i = 0; i < rightEqCount; ++i)
            {
                items.SwapIndexed(l + i, nextEqRightIdx + 1 + i);
            }

            int eqStartIndex = r - leftEqCount + 1;
            int gtStartIndex = l + rightEqCount;
            return new ThreeWayPartitionResult(eqStartIndex, gtStartIndex);
        }
    }
}
