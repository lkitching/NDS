using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Algorithms.Sorting
{
    public class ShellSort : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            for (int h = GetH(fromIndex, toIndex); h > 0; h /= 3)
            {
                for (int i = fromIndex + h; i <= toIndex; ++i)
                {
                    int j = i;
                    T v = items[i];
                    while (j >= fromIndex + h && comp.Compare(v, items[j - h]) < 0)
                    {
                        items[j] = items[j - h];
                        j -= h;
                    }
                    items[j] = v;
                }
            }
        }

        private static int GetH(int fromIndex, int toIndex)
        {
            int h;
            for (h = 1; h <= (toIndex - fromIndex) / 9; h = 3 * h + 1) ;
            return h;
        }
    }
}
