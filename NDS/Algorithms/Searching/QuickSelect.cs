using System.Collections.Generic;
using System.Diagnostics.Contracts;

using NDS.Algorithms.Sorting;

namespace NDS.Algorithms.Searching
{
    /// <summary>Implementation of the quickselect algorithm using a three-way partition.</summary>
    public static class QuickSelect
    {
        /// <summary>Returns the kth-smallest element of a list according to the given comparer.</summary>
        /// <typeparam name="T">Element type of the input list.</typeparam>
        /// <param name="items">The list to search. The elements of the list may be re-ordered during the search.</param>
        /// <param name="k">The smallest index to search for.</param>
        public static T KthSmallest<T>(IList<T> items, int k)
        {
            return KthSmallest(items, k, Comparer<T>.Default);
        }

        /// <summary>Returns the kth-smallest element of a list according to the given comparer.</summary>
        /// <typeparam name="T">Element type of the input list.</typeparam>
        /// <param name="items">The list to search. The elements of the list may be re-ordered during the search.</param>
        /// <param name="k">The smallest index to search for.</param>
        /// <param name="comparer">Comparer for comparing elements in the list.</param>
        /// <returns>The kth-smallest item in <paramref name="items"/> according to <paramref name="comparer"/>.</returns>
        public static T KthSmallest<T>(IList<T> items, int k, IComparer<T> comparer)
        {
            Contract.Requires(items != null);
            Contract.Requires(items.IsReadOnly == false);
            Contract.Requires(comparer != null);
            Contract.Requires(k >= 0 && k < items.Count);

            int left = 0;
            int right = items.Count;

            while(IntRange.RangeCount(left, right) > 1)
            {
                var partitionResult = ThreeWayPartition.Partition(items, left, right, comparer);
                if(k < partitionResult.EqStartIndex)
                {
                    //k < pivot index so continue search in range [left, EqStartIndex)
                    right = partitionResult.EqStartIndex;
                }
                else if(k < partitionResult.GtStartIndex)
                {
                    //k within equal partition so return any item within it
                    return items[partitionResult.EqStartIndex];
                }
                else
                {
                    //k 
                    left = partitionResult.GtStartIndex;
                }
            }

            //only one element left in the range so must be at left
            return items[left];
        }
    }
}
