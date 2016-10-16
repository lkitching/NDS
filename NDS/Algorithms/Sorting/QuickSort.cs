using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NDS.Algorithms.Sorting
{
    /// <summary>In-place quick sort implementation.</summary>
    public class QuickSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            //done if range contains one or zero items
            if (IntRange.RangeCount(fromIndex, toIndex) <= 1) return;

            //int i = Partition(items, fromIndex, toIndex, comp);
            //int eqStartIndex, gtStartIndex;
            //int i = ThreeWayPartition(items, fromIndex, toIndex, comp, out eqStartIndex, out gtStartIndex);
            var partitionResult = ThreeWayPartition.Partition(items, fromIndex, toIndex, comp);

            //items in range [fromIndex, EqStartIndex) are < pivot
            //items in range [EqStartIndex, GtStartIndex) are = pivot
            //items in range [GtStartIndex, toIndex) are > pivot
            //only need to sort regions with elements != to pivot
            SortRange(items, fromIndex, partitionResult.EqStartIndex, comp);
            SortRange(items, partitionResult.GtStartIndex, toIndex, comp);
        }
    }
}
