namespace NDS
{
    /// <summary>
    /// Represents a collection of disjoin sets of type <typeparamref name="T"/>. Each set has a
    /// representative items which can be retrieved for each item of the set. Sets can be merged
    /// by any of their contained elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public interface IDisjointSet<T>
    {
        /// <summary>
        /// Finds the representative member of the set containing <paramref name="item"/> 
        /// if it is contained in any of the sets.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>
        /// The representative member of the set containing <paramref name="item"/> 
        /// or None if <paramref name="item"/> is not in any of the sets.
        /// </returns>
        Maybe<T> FindRepresentative(T item);

        /// <summary>
        /// Merges the two sets containing <paramref name="i"/> and <paramref name="i"/>. 
        /// After this operation both items will have the same representative.
        /// </summary>
        /// <param name="i">First item.</param>
        /// <param name="j">Second item.</param>
        /// <exception cref="ArgumentException">
        /// If either <paramref name="i"/> or <paramref name="j"/> do not exist in any set.
        /// </exception>
        void Merge(T i, T j);
    }
}
