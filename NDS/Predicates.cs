using System;
using System.Collections.Generic;

namespace NDS
{
    /// <summary>Utility class for creating predicates.</summary>
    public static class Predicates
    {
        /// <summary>Creates a predicate which returns whether an input is equal to the given value according to the supplied comparer.</summary>
        /// <typeparam name="T">The item type to match.</typeparam>
        /// <param name="value">The value to match.</param>
        /// <param name="comp">The comparer to compare items with <paramref name="value"/>. If it is not supplied, the default comparer for <typeparamref name="T"/> will be used.</param>
        /// <returns>A predicate which returns whether the input is equal to <paramref name="value"/> according to <paramref name="comp"/>.</returns>
        public static Func<T, bool> CreateEqualTo<T>(T value, IEqualityComparer<T> comp = null)
        {
            comp = comp ?? EqualityComparer<T>.Default;
            return i => comp.Equals(value, i);
        }
    }
}
