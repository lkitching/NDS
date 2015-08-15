namespace NDS
{
    /// <summary>
    /// Represents a priority queue which allows insert and querying and removal of the minimum element
    /// according to a given comparison methods for objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of items in this queue.</typeparam>
    public interface IPriorityQueue<T>
    {
        /// <summary>Inserts an element into this queue.</summary>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="InvalidOperationException">If this queue has limited capacity which has been reached.</exception>
        void Insert(T item);

        /// <summary>Gets the minimum value in this queue according to its comparison method.</summary>
        /// <returns>The minimum element in this queue.</returns>
        /// <exception cref="InvalidOperationException">If this queue is empty.</exception>
        T GetMinimum();

        /// <summary>Removes the minimum element from this queue.</summary>
        /// <returns>The removed minimum element.</returns>
        /// <exception cref="InvalidOperationException">If this queue is empty.</exception>
        T RemoveMinimum();

        /// <summary>Gets the number of items in this queue.</summary>
        int Count { get; }

        /// <summary>Removes all the elements from this queue.</summary>
        void Clear();
    }
}
