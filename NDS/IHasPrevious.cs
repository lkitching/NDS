using System;
using System.Collections.Generic;

namespace NDS
{
    /// <summary>Represents an object which has a predecessor.</summary>
    /// <typeparam name="TPrevious">The type of the successor item.</typeparam>
    public interface IHasPrevious<out TPrevious> where TPrevious : class
    {
        /// <summary>Gets the predecessor for this object. Returns null if there is no predecessor.</summary>
        TPrevious Previous { get; }
    }

    /// <summary>Extension methods for <see cref="IHasNext{TNext}"/>.</summary>
    public static class HasPreviousExtensions
    {
        /// <summary>Enumerates all the items from the given object.</summary>
        /// <typeparam name="TPrevious"></typeparam>
        /// <param name="node">The node to start enumerating from.</param>
        /// <returns>A sequence containing <paramref name="node"/> and all its successors.</returns>
        public static IEnumerable<TPrevious> EnumerateBackFrom<TPrevious>(this TPrevious node) where TPrevious : class, IHasPrevious<TPrevious>
        {
            return Seq.IterateWhile(node, n => n != null, n => n.Previous);
        }
    }
}
