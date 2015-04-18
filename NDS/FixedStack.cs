using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents a stack with a bounded capacity.</summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class FixedStack<T> : IStack<T>
    {
        private T[] items;
        private int topIndex = -1;

        /// <summary>Creates a new instance of this class with the given capacity.</summary>
        /// <param name="capacity">The capacity of this stack.</param>
        public FixedStack(int capacity)
        {
            Contract.Requires(capacity >= 0);
            this.items = new T[capacity];
        }

        /// <see cref="IStack{T}.Pop"/>
        public T Peek()
        {
            this.GuardNotEmpty();
            return this.items[this.topIndex];
        }

        /// <see cref="IStack{T}.Pop"/>
        public T Pop()
        {
            this.GuardNotEmpty();

            T removed = this.items[this.topIndex];
            this.items[this.topIndex] = default(T);
            this.topIndex--;

            return removed;
        }

        /// <summary>Pushes an item on this stack if there is sufficient capacity.</summary>
        /// <param name="item">The item to push.</param>
        /// <exception cref="InvalidOperationException">If this stack is full.</exception>
        public void Push(T item)
        {
            if (this.topIndex == this.items.Length - 1) throw new InvalidOperationException("Insufficient capacity");
            this.topIndex++;
            this.items[this.topIndex] = item;
        }

        /// <see cref="IStack{T}.Pop"/>
        public int Count
        {
            get { return this.topIndex + 1; }
        }

        private void GuardNotEmpty()
        {
            if (this.topIndex == -1) throw new InvalidOperationException("Stack is empty");
            else
            {
                Debug.Assert(this.topIndex >= 0 && this.topIndex < this.items.Length);
            }
        }
    }
}
