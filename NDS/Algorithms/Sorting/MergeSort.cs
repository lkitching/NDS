using System;
using System.Collections.Generic;

namespace NDS.Algorithms.Sorting
{
    /// <summary>In-place merge sort.</summary>
    public class MergeSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            //done if range contains one or zero items
            if (IntRange.RangeCount(fromIndex, toIndex) <= 1) return;

            int midIndex = IntRange.RangeMidpoint(fromIndex, toIndex);

            //sort two halves of the input
            SortRange(items, fromIndex, midIndex + 1, comp);
            SortRange(items, midIndex + 1, toIndex, comp);

            Merge(items, fromIndex, midIndex, toIndex, comp);
        }

        /// <summary>
        /// Merges two sorted segments of an array in place. The first range is [fromIndex..midIndex]
        /// and the second [midIndex + 1..toIndx).
        /// </summary>
        /// <typeparam name="T">The element type of the array.</typeparam>
        /// <param name="items">The array to merge.</param>
        /// <param name="fromIndex">Start index of the first range.</param>
        /// <param name="midIndex">End index of the first range.</param>
        /// <param name="toIndex">End index of the second range.</param>
        /// <param name="comp">Comparer for array elements.</param>
        private static void Merge<T>(T[] items, int fromIndex, int midIndex, int toIndex, IComparer<T> comp)
        {
            int rangeLen = IntRange.RangeCount(fromIndex, toIndex);
            T[] tmp = new T[rangeLen];
            for (int i = 0, l = fromIndex, r = midIndex + 1; i < tmp.Length; ++i)
            {
                if (l > midIndex)
                {
                    //finished merging left so copy from right
                    tmp[i] = items[r];
                    ++r;
                }
                else if (r >= toIndex)
                {
                    //finished merging right so copy from left
                    tmp[i] = items[l];
                    ++l;
                }
                else
                {
                    //items remain in both left and right so copy smaller
                    if (comp.CompareResult(items[l], items[r]) == ComparisonResult.Less)
                    {
                        //copy from left
                        tmp[i] = items[l];
                        ++l;
                    }
                    else
                    {
                        //copy from right
                        tmp[i] = items[r];
                        ++r;
                    }
                }
            }

            Array.Copy(tmp, 0, items, fromIndex, tmp.Length);
        }
    }
}
