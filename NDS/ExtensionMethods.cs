using System;
using System.Collections.Generic;
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
    }
}
