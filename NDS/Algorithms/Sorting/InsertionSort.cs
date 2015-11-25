using System;
using System.Collections.Generic;

namespace NDS.Algorithms.Sorting
{
    public class InsertionSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            for (int i = fromIndex + 1; i <= toIndex; ++i)
            {
                //invariant: [fromIndex..i] is sorted
                int j = i;

                //copy current item and make room for it in the sorted sub-array [fromIndex..i]
                T tmp = items[j];

                //move every item greater than the current item to the right
                while (j > fromIndex && comp.CompareResult(tmp, items[j - 1]) == ComparisonResult.Less)
                {
                    items[j] = items[j - 1];
                    j--;
                }

                items[j] = tmp;
            }
        }
    }
}
