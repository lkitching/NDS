using System;
using System.Collections.Generic;

namespace NDS
{
    /// <summary>Represents an object which has a successor.</summary>
    /// <typeparam name="TNext">The type of the successor item.</typeparam>
    public interface IHasNext<out TNext> where TNext : class
    {
        /// <summary>Gets the successor for this object. Returns null if there is no successor.</summary>
        TNext Next { get; }
    }

    /// <summary>Extension methods for <see cref="IHasNext{TNext}"/>.</summary>
    public static class HasNextExtensions
    {
        /// <summary>Enumerates all the items from the given object.</summary>
        /// <typeparam name="TNext"></typeparam>
        /// <param name="node">The node to start enumerating from.</param>
        /// <returns>A sequence containing <paramref name="node"/> and all its successors.</returns>
        public static IEnumerable<TNext> EnumerateFrom<TNext>(this TNext node) where TNext : class, IHasNext<TNext>
        {
            return Seq.IterateWhile(node, n => n != null, n => n.Next);
        }
    }
}
