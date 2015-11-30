using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Algorithms.Searching
{
    public class BinarySearch
    {
        public static BinarySearchResult Search<T>(T[] items, int fromIndex, int toIndex, T item, IComparer<T> comp)
        {
            int result = fromIndex;

            while (toIndex >= fromIndex)
            {
                int midIndex = (fromIndex + toIndex) / 2;
                T mid = items[midIndex];
                var compResult = comp.CompareResult(item, mid);

                switch (compResult)
                {
                    case ComparisonResult.Greater:
                        fromIndex = midIndex + 1;
                        result = midIndex;
                        break;
                    case ComparisonResult.Less:
                        toIndex = midIndex - 1;
                        result = midIndex;
                        break;
                    default:
                        return new BinarySearchResult(midIndex, true);
                }
            }

            return new BinarySearchResult(result, false);
        }
    }
}
