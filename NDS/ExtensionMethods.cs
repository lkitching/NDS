using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    public static class ExtensionMethods
    {
        public static TValue GetOr<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            TValue val;
            return dict.TryGetValue(key, out val) ? val : @default;
        }

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return GetOr(dict, key, default(TValue));
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> seq)
        {
            return seq ?? Enumerable.Empty<T>();
        }

        public static ComparisonResult ToComparisonResult(this int i)
        {
            if (i < 0) return ComparisonResult.Less;
            else if (i > 0) return ComparisonResult.Greater;
            else return ComparisonResult.Equal;
        }

        public static ComparisonResult CompareResult<T>(this IComparer<T> comparer, T x, T y)
        {
            return comparer.Compare(x, y).ToComparisonResult();
        }

        public static void SwapIndexed<T>(this T[] items, int i, int j)
        {
            T tmp = items[i];
            items[i] = items[j];
            items[j] = tmp;
        }

        public static void SwapIndexedWhen<T>(this T[] items, IComparer<T> comparer, int i, int j, ComparisonResult whenResult)
        {
            if (comparer.CompareResult(items[i], items[j]) == whenResult)
            {
                SwapIndexed(items, i, j);
            }
        }

        public static IComparer<T> Reverse<T>(this IComparer<T> comp)
        {
            Contract.Requires(comp != null);
            return new ReverseComparer<T>(comp);
        }

        private class ReverseComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> inner;

            public ReverseComparer(IComparer<T> inner)
            {
                this.inner = inner;
            }

            public int Compare(T x, T y)
            {
                return this.inner.Compare(y, x);
            }
        }
    }
}
