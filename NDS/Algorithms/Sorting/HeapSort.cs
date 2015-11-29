using System;
using System.Collections.Generic;

namespace NDS.Algorithms.Sorting
{
    /// <summary>In-place implementation of heap sort.</summary>
    public class HeapSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            var maxComp = comp.Reverse();
            
            //work back through the range fixing each element down through the tree
            //this constructs the max heap in place in the range
            for (int i = toIndex; i >= fromIndex; --i)
            {
                BinaryHeapOperations.FixDown(items, i, fromIndex, toIndex, maxComp);
            }

            //continually move the max item in the heap (the first item) into position at
            //the end of the range. Swap with the current end element to put it in place then
            //fix up the heap to move the next-largest item to the front
            int unsortedEndIndex = toIndex;
            while (unsortedEndIndex > fromIndex)
            {
                items.SwapIndexed(fromIndex, unsortedEndIndex);

                unsortedEndIndex--;
                BinaryHeapOperations.FixDown(items, fromIndex, fromIndex, unsortedEndIndex, maxComp);
            }
        }

    }
}
