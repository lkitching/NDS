using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Utility class for generating sequences.</summary>
    public static class Seq
    {
        /// <summary>Generates a sequence from the given initial value until the given predicate returns false.</summary>
        /// <typeparam name="T">The type of items in the generated sequence.</typeparam>
        /// <param name="initial">The initial value in the sequence.</param>
        /// <param name="cond">Predicate to test items against. The iteration ends if this predicate returns false for the current item.</param>
        /// <param name="next">Used to generate the next item in the sequence from the current value.</param>
        /// <returns>A sequence containing all the items generated from the repeated application of <paramref name="next"/> which pass <paramref name="cond"/>.</returns>
        public static IEnumerable<T> IterateWhile<T>(T initial, Func<T, bool> cond, Func<T, T> next)
        {
            Contract.Requires(cond != null);
            Contract.Requires(next != null);

            T current = initial;
            while (cond(current))
            {
                yield return current;
                current = next(current);
            }
        }
    }
}
