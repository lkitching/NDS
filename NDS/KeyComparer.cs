using System;
using System.Collections.Generic;

namespace NDS
{
    /// <summary>Comparer which compares items by their associated keys.</summary>
    /// <typeparam name="T">The item types.</typeparam>
    /// <typeparam name="TKey">The keys to compare items by.</typeparam>
    public class KeyComparer<T, TKey> : IComparer<T>
    {
        private readonly Func<T, TKey> keyFunc;
        private readonly IComparer<TKey> keyComparer;

        /// <summary>Creates a new instance with the given key extractor function.</summary>
        /// <param name="keyFunc">Delegate which returns the key for the given item.</param>
        public KeyComparer(Func<T, TKey> keyFunc)
            : this(keyFunc, Comparer<TKey>.Default)
        {
        }

        /// <summary>Creates a new instance with the given key extractor function.</summary>
        /// <param name="keyFunc">Delegate which returns the key for the given item.</param>
        /// <param name="keyComp">Comparer for keys.</param>
        public KeyComparer(Func<T, TKey> keyFunc, IComparer<TKey> keyComp)
        {
            this.keyFunc = keyFunc;
            this.keyComparer = keyComp;
        }

        /// <summary>Compares two items by their keys.</summary>
        /// <param name="x">The first item.</param>
        /// <param name="y">The second item.</param>
        /// <returns>Comparison result for the keys of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public int Compare(T x, T y)
        {
            return this.keyComparer.Compare(this.keyFunc(x), this.keyFunc(y));
        }
    }

    public static class KeyComparer
    {
        /// <summary>Creates a comparer which compares items by their associated keys.</summary>
        /// <typeparam name="T">The type of items to compare.</typeparam>
        /// <typeparam name="TKey">The key type to compare by.</typeparam>
        /// <param name="keyFunc">Delegate which returns the key for an item.</param>
        /// <returns>Comparer for items of type <typeparamref name="T"/> by their keys.</returns>
        public static KeyComparer<T, TKey> Create<T, TKey>(Func<T, TKey> keyFunc) where TKey : IComparable<TKey>
        {
            return new KeyComparer<T, TKey>(keyFunc);
        }
    }
}
