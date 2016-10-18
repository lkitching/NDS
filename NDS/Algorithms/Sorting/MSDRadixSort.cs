using System;
using System.Linq;

namespace NDS.Algorithms.Sorting
{
    /// <summary>Sorts the items within the input from the most significant byte to the least.</summary>
    public class MSDRadixSort : IRadixSort
    {
        public void RadixSort<T>(T[] items, IByteAddressable<T> ops, int fromIndex, int toIndex)
        {
            var aux = new T[IntRange.RangeCount(fromIndex, toIndex)];
            Sort(items, aux, ops, fromIndex, toIndex, 0);
        }

        private static void Sort<T>(T[] items, T[] aux, IByteAddressable<T> ops, int fromIndex, int toIndex, int msbIndex)
        {
            //done if all key bytes compared
            if (msbIndex >= ops.NumBytes) return;

            var range = new IntRange(fromIndex, toIndex);

            //TODO: insertion sort small ranges
            if (range.Count <= 1) return;

            //radix = 256
            var hist = new int[257];

            //calculate counts of each byte at index msbIndex
            for (int i = fromIndex; i < toIndex; ++i)
            {
                T item = items[i];
                hist[ops.GetByte(item, msbIndex) + 1]++;
            }

            //calculate the cumulative sums for the number of keys where keyBytes[msbIndex] < i for each byte [0..256]
            //this will contain the starting index for items with key i in the sorted input array
            for(int i = 1; i < hist.Length; ++i)
            {
                hist[i] += hist[i - 1];
            }

            var partitions = hist.ToArray();

            //move each item in the input into its sorted position in the auxiliary array
            //as item is moved into place, the starting index for the key is moved to the right
            for(int i = fromIndex; i < toIndex; ++i)
            {
                byte keyByte = ops.GetByte(items[i], msbIndex);
                int targetIndex = hist[keyByte];
                aux[targetIndex] = items[i];
                hist[keyByte]++;
            }

            //items should have been copied into [0, ... rangeCount) within aux so copy
            //back into target range in the source array
            Array.Copy(aux, 0, items, fromIndex, range.Count);

            //recursively sort each bucket
            for(int i = 1; i < partitions.Length; ++i)
            {
                Sort(items, aux, ops, fromIndex + partitions[i - 1], fromIndex + partitions[i], msbIndex + 1);
            }
        }
    }
}
