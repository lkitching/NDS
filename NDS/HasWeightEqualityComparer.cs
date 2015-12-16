using System.Collections.Generic;

namespace NDS
{
    /// <summary>Extends an equality type for some type to account for its weight.</summary>
    /// <typeparam name="T">The enclosing type.</typeparam>
    /// <typeparam name="W">The weight type.</typeparam>
    public class HasWeightEqualityComparer<T, W> : IEqualityComparer<T>
        where T : IHasWeight<W>
    {
        private readonly IEqualityComparer<T> restComparer;
        private readonly IEqualityComparer<W> weightComparer;

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="restComparer">Equality comparer for <typeparamref name="T"/> which ignores the weight.</param>
        /// <param name="weightComparer">Equality comparer for the weight.</param>
        public HasWeightEqualityComparer(IEqualityComparer<T> restComparer, IEqualityComparer<W> weightComparer)
        {
            this.restComparer = restComparer;
            this.weightComparer = weightComparer;
        }

        public bool Equals(T x, T y)
        {
            return restComparer.Equals(x, y) && this.weightComparer.Equals(x.Weight, y.Weight);
        }

        public int GetHashCode(T obj)
        {
            return (11 * this.restComparer.GetHashCode(obj)) + this.weightComparer.GetHashCode(obj.Weight);
        }
    }
}
