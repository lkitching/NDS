using System;
using System.Collections.Generic;

namespace NDS.Algorithms.Sorting
{
    public class SelectionSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            for (int i = fromIndex; i < toIndex; ++i)
            {
                int minIndex = i;
                //find the index of the minimum item in the range [i..toIndex]
                //then swap it with the item at index i
                for (int j = i + 1; j <= toIndex; ++j)
                {
                    if (comp.Compare(items[j], items[minIndex]) < 0)
                    {
                        minIndex = j;
                    }
                }

                items.SwapIndexed(i, minIndex);
            }
        }
    }
}
