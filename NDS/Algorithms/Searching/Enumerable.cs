using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Algorithms.Searching
{
    public static class Enumerable
    {
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
