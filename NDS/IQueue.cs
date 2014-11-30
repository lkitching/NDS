using System;
using System.Collections.Generic;

namespace NDS
{
    /// <summary>Represents a first-in first-out data structure.</summary>
    /// <typeparam name="T"></typeparam>
    public interface IQueue<T> : IEnumerable<T>
    {
        /// <summary>Adds an item to the back of this queue.</summary>
        /// <param name="item">The item to enqueue.</param>
        /// <exception cref="InvalidOperationException">If this queue does not currently allow enqueuing i.e. CanEnqueue returns false.</exception>
        void Enqueue(T item);

        /// <summary>Indicates whether this queue currently allows an item to be enqueued.</summary>
        bool CanEnqueue { get; }

        /// <summary>Removes the item at the front of this queue.</summary>
        /// <returns>The removed it.</returns>
        /// <exception cref="InvalidOperationException">If this queue is empty.</exception>
        T Dequeue();

        /// <summary>Gets the number of items in this queue.</summary>
        int Count { get; }
    }
}
