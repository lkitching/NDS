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

        /// <summary>Returns whether the input sequence is sorted in ascending order.</summary>
        /// <typeparam name="T">The items of the input sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <returns>Whether <paramref name="seq"/> is in ascending order by the natural comparison on <typeparamref name="T"/>.</returns>
        public static bool IsSorted<T>(this IEnumerable<T> seq) where T : IComparable<T>
        {
            return IsSortedBy(seq, Comparer<T>.Default);
        }

        /// <summary>Returns whether the input sequence is in ascending order according to the given comparer.</summary>
        /// <typeparam name="T">The type of items in the sequence.</typeparam>
        /// <param name="seq">The input sequence.</param>
        /// <param name="comp">Comparer for the items of <paramref name="seq"/>.</param>
        /// <returns>Whether <paramref name="seq"/> is ordered in ascending order according to <paramref name="comp"/>.</returns>
        public static bool IsSortedBy<T>(this IEnumerable<T> seq, IComparer<T> comp)
        {
            using (var enumerator = seq.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    T prev = enumerator.Current;
                    while(enumerator.MoveNext())
                    {
                        T current = enumerator.Current;
                        if (comp.Compare(prev, current) > 0) return false;

                        prev = current;
                    }

                    return true;
                }
                else return true;
            }
        }

        public static Maybe<T> MinimumBy<T, TKey>(this IEnumerable<T> seq, Func<T, TKey> keyFunc, IComparer<TKey> keyComparer)
        {
            var min = Maybe.None<KeyValuePair<T, TKey>>();
            foreach (T item in seq)
            {
                if (min.HasValue)
                {
                    TKey currentKey = keyFunc(item);
                    if (keyComparer.CompareResult(currentKey, min.Value.Value) == ComparisonResult.Less)
                    {
                        min = Maybe.Some(new KeyValuePair<T, TKey>(item, currentKey));
                    }
                }
                else
                {
                    min = Maybe.Some(new KeyValuePair<T, TKey>(item, keyFunc(item)));
                }
            }

            return min.Select(kvp => kvp.Key);
        }
    }
}
