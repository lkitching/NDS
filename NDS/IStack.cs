using System;

namespace NDS
{
    /// <summary>Collection supporing last-in-first-out removal.</summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IStack<T>
    {
        /// <summary>Gets the item on the top of the stack.</summary>
        /// <returns>The item on the top of the stack.</returns>
        /// <exception cref="InvalidOperationException">If the stack is empty.</exception>
        T Peek();

        /// <summary>Pops the item on the top of the stack.</summary>
        /// <returns>The old top of the stack.</returns>
        /// <exception cref="InvalidOperationException">If the stack is empty.</exception>
        T Pop();

        /// <summary>Pushes an item on the top of the stack.</summary>
        /// <param name="item">The item to push.</param>
        /// <exception cref="InvalidOperationException">
        /// Implementations may throw this exception if they have a bounded capacity which has been reached before this operation is executed.
        /// </exception>
        void Push(T item);

        /// <summary>Gets the number of items in this collection.</summary>
        int Count { get; }
    }
}
