using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>An associative container between the given key and value types.</summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IMap<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        /// <summary>Gets the value mapped to the given key if one exists.</summary>
        /// <param name="key">The key to search for.</param>
        /// <returns></returns>
        Maybe<TValue> Get(TKey key);

        /// <summary>Tries to add an association between the given key and value in this map. Returns false if the key is already mapped to a value.</summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to associate with <paramref name="key"/>.</param>
        /// <returns>True if the mapping was added, false if <paramref name="key"/> is already mapped to a value.</returns>
        bool TryAdd(TKey key, TValue value);

        /// <summary>Associates the given key and value in this map. Replaces the current mapped value if one already exists for <paramref name="key"/>.</summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to assoicate with <paramref name="key"/>.</param>
        void Assoc(TKey key, TValue value);

        /// <summary>Deletes the key from this map if it is present.</summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the key existed prior to this operation.</returns>
        bool Delete(TKey key);

        /// <summary>Clears all values from this map.</summary>
        void Clear();

        /// <summary>Gets the number of associations in this map.</summary>
        int Count { get; }
    }

    /// <summary>Extension methods for <see cref="IMap{TKey, TValue}"/>.</summary>
    public static class MapExtensions
    {
        /// <summary>Tries to add an association between the given key and value in this map. Throws an exception if the key is already present.</summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="map">The map to add to.</param>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to associate with <paramref name="key"/>.</param>
        /// <exception cref="InvalidOperationException">If <paramref name="key"/> already exists in <paramref name="map"/>.</exception>
        public static void Add<TKey, TValue>(this IMap<TKey, TValue> map, TKey key, TValue value)
        {
            Contract.Requires(map != null);
            bool added = map.TryAdd(key, value);
            if (!added)
            {
                var message = string.Format("A value already exists for key {0} in this map", key);
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>Adds the given pair to the given map.</summary>
        /// <typeparam name="TKey">The key type for the map.</typeparam>
        /// <typeparam name="TValue">The value type for the map.</typeparam>
        /// <param name="map">The map to add the pair into.</param>
        /// <param name="kvp">The pair to add. Throws an exception if the given key is already associated with a value in this map.</param>
        public static void Add<TKey, TValue>(this IMap<TKey, TValue> map, KeyValuePair<TKey, TValue> kvp)
        {
            Contract.Requires(map != null);
            map.Add(kvp.Key, kvp.Value);
        }

        /// <summary>Associates a pair in the given map.</summary>
        /// <typeparam name="TKey">The key type for the map.</typeparam>
        /// <typeparam name="TValue">The value type for the map.</typeparam>
        /// <param name="map">The map to associate the given key-value pair in.</param>
        /// <param name="kvp">The pair to associate in <paramref name="map"/>.</param>
        public static void Assoc<TKey, TValue>(this IMap<TKey, TValue> map, KeyValuePair<TKey, TValue> kvp)
        {
            Contract.Requires(map != null);
            map.Assoc(kvp.Key, kvp.Value);
        }

        /// <summary>Indicates whether an association exists for the given key in a map.</summary>
        /// <typeparam name="TKey">The key type for the map.</typeparam>
        /// <typeparam name="TValue">The value type for the map.</typeparam>
        /// <param name="map">The map to search.</param>
        /// <param name="key">The key to search for.</param>
        /// <returns>True if <paramref name="key"/> is currently associated with a value in <paramref name="map"/>.</returns>
        public static bool ContainsKey<TKey, TValue>(this IMap<TKey, TValue> map, TKey key)
        {
            Contract.Requires(map != null);
            return map.Get(key).HasValue;
        }
    }
}
