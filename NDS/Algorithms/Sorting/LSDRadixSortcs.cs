using System;

namespace NDS.Algorithms.Sorting
{
    /// <summary>
    /// Sorts an input collection by successively sorting the input from the least to the most significant byte.
    /// </summary>
    public class LSDRadixSort : IRadixSort
    {
        public void RadixSort<T>(T[] items, IByteAddressable<T> ops, int fromIndex, int toIndex)
        {
            int rangeLength = IntRange.RangeCount(fromIndex, toIndex);

            if (rangeLength <= 1) return;

            T[] temp = new T[rangeLength];
            var counts = new int[257];

            for (int byteIndex = ops.NumBytes - 1; byteIndex >= 0; --byteIndex)
            {
                //populate counts
                foreach (T item in items)
                {
                    counts[ops.GetByte(item, byteIndex) + 1]++;
                }

                //calculate cumulative counts
                counts[0] = 0;

                for (int i = 1; i < counts.Length; ++i)
                {
                    counts[i] += counts[i - 1];
                }

                foreach (T item in items)
                {
                    var b = ops.GetByte(item, byteIndex);
                    temp[counts[b]] = item;
                    counts[b]++;
                }

                //copy items back into source array
                Array.Copy(temp, 0, items, fromIndex, rangeLength);

                Array.Clear(counts, 0, counts.Length);
            }
        }
    }
}
