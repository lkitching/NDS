using System;
using System.Collections.Generic;

namespace NDS.Algorithms.Sorting
{
    /// <summary>In-place quick sort implementation.</summary>
    public class QuickSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            //done if range contains one or zero items
            if (IntRange.RangeCount(fromIndex, toIndex) <= 1) return;

            int i = Partition(items, fromIndex, toIndex, comp);
            SortRange(items, fromIndex, i, comp);
            SortRange(items, i + 1, toIndex, comp);
        }

        /// <summary>
        /// Chooses a pivot from the range [fromIndex..toIndex) and partitions the array around that
        /// element. After the partition, all items in the range [fromIndex..ret-1] are equal to or less
        /// than the pivot, and all items in the range [ret+1..toIndex) are equal or greater.
        /// </summary>
        /// <typeparam name="T">Element type of the collection.</typeparam>
        /// <param name="items">The collection to partition.</param>
        /// <param name="fromIndex">Start index of the range to partition.</param>
        /// <param name="toIndex">End index of the range to partition.</param>
        /// <param name="comp">Comparer for the elements in the collection.</param>
        /// <returns>The index of the pivot in the partitioned array.</returns>
        private static int Partition<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            //choose last item in range as the pivot
            int pivotIndex = toIndex - 1;
            T pivot = items[pivotIndex];
            int l = fromIndex;
            int r = pivotIndex - 1;

            while (true)
            {
                //find the next element from the left greater or equal to the pivot
                //NOTE: will not run out of bounds since pivot is not less than itself
                while (comp.CompareResult(items[l], pivot) == ComparisonResult.Less)
                {
                    ++l;
                }

                //find the next item from the right less than or equal to the pivot
                while (comp.CompareResult(items[r], pivot) == ComparisonResult.Greater)
                {
                    //NOTE: done if we cross into the left side of the partition
                    if (l == r) break;
                    --r;
                }

                if (l >= r) break;

                //item at items[l] is greater than the pivot and the element at items[r] is less
                //so exchange them to put them both on the correct side of the pivot
                items.SwapIndexed(l, r);
            }

            //finally put the pivot into its sorted position in the input
            items.SwapIndexed(l, pivotIndex);
            return l;
        }
    }
}
