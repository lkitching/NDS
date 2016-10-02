using System;
using System.Collections.Generic;

namespace NDS.Algorithms.Sorting
{
    public class BubbleSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            for (int i = fromIndex; i < toIndex; ++i)
            {
                //'bubble' the smallest element in the range [i..toIndex) into its sorted position
                for (int j = toIndex - 1; j > i; --j)
                {
                    items.SwapIndexedWhen(comp, j, j - 1, ComparisonResult.Less);
                }

                //NOTE: element at items[i] is now in its sorted position
            }
        }
    }
}
