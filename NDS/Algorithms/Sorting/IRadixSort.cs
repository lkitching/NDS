namespace NDS.Algorithms.Sorting
{
    /// <summary>Implementation of a radix sort.</summary>
    public interface IRadixSort
    {
        /// <summary>Sorts the range [fromIndex, toIndex) of the input using radix sort.</summary>
        /// <typeparam name="T">The type of items in the input collection.</typeparam>
        /// <param name="items">The collection to sort.</param>
        /// <param name="addr">used to access individual bytes within each item.</param>
        /// <param name="fromIndex">Start index of the range to sort.</param>
        /// <param name="toIndex">Exclusive index of the end of the range to sort.</param>
        void RadixSort<T>(T[] items, IByteAddressable<T> addr, int fromIndex, int toIndex);
    }
}
