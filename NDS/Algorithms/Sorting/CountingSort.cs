using System;
using System.Diagnostics.Contracts;

namespace NDS.Algorithms.Sorting
{
    public class CountingSort
    {
        /// <summary>
        /// Sorts a collection of items by their associated integer keys. All keys should
        /// be >= 0 and the maximum key should ideally be small.
        /// </summary>
        /// <typeparam name="T">The type of items to sort.</typeparam>
        /// <param name="items">The collection to sort.</param>
        /// <param name="keyFunc">Delegate to return the key for a given item.</param>
        public void Sort<T>(T[] items, Func<T, int> keyFunc)
        {
            Contract.Requires(Contract.ForAll(items, i => keyFunc(i) >= 0));

            if (items.Length > 1)
            {
                int maxKey = keyFunc(items[0]);
                for(int i = 1; i < items.Length; ++i)
                {
                    T item = items[i];
                    int key = keyFunc(item);
                    if(key > maxKey)
                    {
                        maxKey = key;
                    }
                }

                Sort(items, keyFunc, maxKey);
            }
        }

        /// <summary>
        /// Sorts a collection of items by their associated integer keys. Keys should be in the 
        /// range [0, maxKey] which should be small.
        /// </summary>
        /// <typeparam name="T">The type of items to sort.</typeparam>
        /// <param name="items">The collection to sort.</param>
        /// <param name="keyFunc">Delegate to return the key for a given item.</param>
        /// <param name="maxKey">The maximum key for all elements in <paramref name="items"/>.</param>
        public void Sort<T>(T[] items, Func<T, int> keyFunc, int maxKey)
        {
            Contract.Requires(maxKey >= 0);
            Contract.Requires(Contract.ForAll(items, i => keyFunc(i) >= 0 && keyFunc(i) <= maxKey));

            if (items.Length <= 1) return;

            var hist = new int[maxKey + 1];

            //calculate counts of each key in the input
            foreach(T item in items)
            {
                hist[keyFunc(item)]++;
            }

            //calculate the cumulative sums for the number of keys less than i for each index [0..maxKey]
            //this will contain the starting index for items with key i in the sorted input array
            int count = 0;
            for(int i = 0; i < hist.Length; ++i)
            {
                int current = hist[i];
                hist[i] = count;
                count += current;
            }

            //move each item in the input into its sorted position in the auxiliary array
            //as item is moved into place, the starting index for the key is moved to the right
            T[] temp = new T[items.Length];
            foreach(T item in items)
            {
                int key = keyFunc(item);
                temp[hist[key]] = item;
                hist[key]++;
            }

            Array.Copy(temp, items, items.Length);
        }
    }
}
