using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents a collection from which items can be removed.</summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IRemovable<out T>
    {
        /// <summary>Removes the first item in the collection which matches the given predicate.</summary>
        /// <param name="predicate">The predicate to match elements of this collection.</param>
        /// <returns>Whether a matching item was found.</returns>
        bool RemoveFirstWhere(Func<T, bool> predicate);

        /// <summary>Removes all items in this collection matching the given predicate.</summary>
        /// <param name="predicate">The predicate to match elements of this collection.</param>
        /// <returns>The number of removed items.</returns>
        int RemoveAllWhere(Func<T, bool> predicate);
    }

    /// <summary>Extension methods for <see cref="IRemovable{T}"/>.</summary>
    public static class RemovableExtensions
    {
        /// <summary>Removes the first item in the collection equal to the given value according to the supplied comparer.</summary>
        /// <typeparam name="T"The type of items in the collection.</typeparam>
        /// <param name="rem">The collection to remove from.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="comp">The comparer to use to compare items in the collection. If not supplied the default comparer for <typeparam name="T"/> will be used.</param>
        /// <returns>Whether a matching item was found.</returns>
        public static bool RemoveFirst<T>(this IRemovable<T> rem, T value, IEqualityComparer<T> comp = null)
        {
            Contract.Requires(rem != null);
            return rem.RemoveFirstWhere(Predicates.CreateEqualTo(value, comp));
        }

        /// <summary>Removes all items in the collection equal to the given value according to the supplied comparer.</summary>
        /// <typeparam name="T"The type of items in the collection.</typeparam>
        /// <param name="rem">The collection to remove from.</param>
        /// <param name="value">The value to remove.</param>
        /// <param name="comp">The comparer to use to compare items in the collection. If not supplied the default comparer for <typeparam name="T"/> will be used.</param>
        /// <returns>The number of items removed.</returns>
        public static int RemoveAll<T>(this IRemovable<T> rem, T value, IEqualityComparer<T> comp = null)
        {
            Contract.Requires(rem != null);
            return rem.RemoveAllWhere(Predicates.CreateEqualTo(value, comp));
        }
    }
}
