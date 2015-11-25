using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS.Algorithms.Sorting
{
    /// <summary>Represents an algorithm for sorting an indexable collection in place.</summary>
    [ContractClass(typeof(InPlaceSortContracts))]
    public interface IInPlaceSort
    {
        /// <summary>Sorts a range of a collection in place.</summary>
        /// <typeparam name="T">Element type of the collection.</typeparam>
        /// <param name="items">The collection to sort.</param>
        /// <param name="fromIndex">Start index for the range to sort.</param>
        /// <param name="toIndex">Inclusive end index for the range to sort.</param>
        /// <param name="comp">Comparer for the elements of <paramref name="items"/>.</param>
        void SortRange<T>(T [] items, int fromIndex, int toIndex, IComparer<T> comp);
    }

    public static class InPlaceSortExtensions
    {
        /// <summary>Sorts a collection in place.</summary>
        /// <typeparam name="T">Element type of the collection.</typeparam>
        /// <param name="sort">The sorting algorith to use for the sort.</param>
        /// <param name="items">The collection to sort.</param>
        /// <param name="comp">Comparer for the elements of the collection.</param>
        public static void Sort<T>(this IInPlaceSort sort, T[] items, IComparer<T> comp)
        {
            if (items.Length > 1)
            {
                sort.SortRange(items, 0, items.Length - 1, comp);
            }
        }
    }

    [ContractClassFor(typeof(IInPlaceSort))]
    public class InPlaceSortContracts : IInPlaceSort
    {
        public void SortRange<T>(T[] items, int fromIndex, int toIndex, IComparer<T> comp)
        {
            Contract.Requires(items != null);
            Contract.Requires(comp != null);
            Contract.Requires(fromIndex >= 0);
            Contract.Requires(toIndex >= fromIndex);
            Contract.Requires(toIndex < items.Length);
        }
    }
}
