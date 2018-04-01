using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS.Algorithms.Sorting
{
    public static class Median
    {
        /// <summary>Gets the index of the median of the values at three indexes.</summary>
        /// <typeparam name="T">The type of items to compare.</typeparam>
        /// <param name="items">Collection containing the values.</param>
        /// <param name="i">First index</param>
        /// <param name="j">Second index</param>
        /// <param name="k">Third index</param>
        /// <param name="comp">Comparer for items.</param>
        /// <returns>Returns the index of the median of items[i], items[j] and items[k] according to <paramref name="comp"/>.</returns>
        public static int MedianIndex<T>(IList<T> items, int i, int j, int k, IComparer<T> comp)
        {
            if (comp.Compare(items[i], items[j]) < 0)
            {
                if (comp.Compare(items[j], items[k]) < 0)
                {
                    // i <= j <= k
                    return j;
                }
                else
                {
                    //j >= i
                    //j >= k
                    return (comp.Compare(items[i], items[k]) < 0) ? k : i;
                }
            }
            else
            {
                if (comp.Compare(items[i], items[k]) < 0)
                {
                    //j <= i <= k
                    return i;
                }
                else
                {
                    //i >= j
                    //i >= k
                    return (comp.Compare(items[j], items[k]) < 0) ? k : j;
                }
            }
        }

        /// <summary>
        /// Estimates the index of the median item within the range [fromIndex, toIndex) of a collection.
        /// </summary>
        /// <typeparam name="T">The type of items in the array.</typeparam>
        /// <param name="items">Collection to </param>
        /// <param name="fromIndex">Start index of the range to search within <paramref name="items"/>.</param>
        /// <param name="toIndex">Exclusive end index of the range to search.</param>
        /// <param name="comp">Comparer for items.</param>
        /// <returns></returns>
        public static int EstimateMedianIndex<T>(IList<T> items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            Contract.Requires(items != null);
            Contract.Ensures(Contract.Result<int>() >= fromIndex && Contract.Result<int>() < toIndex);

            //Implementation taken from 'Engineering a sort function'
            //http://cs.fit.edu/~pkc/classes/writing/samples/bentley93engineering.pdf

            int midIndex = IntRange.RangeMidpoint(fromIndex, toIndex);
            int rangeLength = IntRange.RangeCount(fromIndex, toIndex);

            if (rangeLength > 40)
            {
                //take 9 items from the range and take median of 3 3-way medians of the values
                int s = rangeLength / 8;
                int maxIndex = toIndex - 1;

                int pl = MedianIndex(items, fromIndex, fromIndex + s, fromIndex + 2 * s, comp);
                int pm = MedianIndex(items, midIndex - s, midIndex, midIndex + s, comp);
                int pn = MedianIndex(items, maxIndex - 2 * s, maxIndex - s, maxIndex, comp);

                return MedianIndex(items, pl, pm, pn, comp);
            }
            else if (rangeLength > 7)
            {
                //medium-size range so take median of start, middle and end
                return MedianIndex(items, fromIndex, midIndex, toIndex - 1, comp);
            }
            else
            {
                //small range so use the middle element
                return midIndex;
            }
        }
    }
}
