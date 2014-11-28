using System;

namespace NDS
{
    /// <summary>Represents an unbounded stack.</summary>
    /// <typeparam name="T">The type of items in this stack.</typeparam>
    public class DynamicStack<T> : IStack<T>
    {
        private SinglyLinkedListCollection<T> items = new SinglyLinkedListCollection<T>();

        /// <see cref="IStack{T}.Peek"/>
        public T Peek()
        {
            this.GuardNotEmpty();
            return this.items.First;
        }

        /// <see cref="IStack{T}.Pop"/>
        public T Pop()
        {
            this.GuardNotEmpty();
            return this.items.RemoveFirst();
        }

        /// <see cref="IStack{T}.Push"/>
        public void Push(T item)
        {
            this.items.AddFirst(item);
        }

        /// <see cref="IStack{T}.Count"/>
        public int Count
        {
            get { return this.items.Count; }
        }

        private void GuardNotEmpty()
        {
            if (this.items.Count == 0) throw new InvalidOperationException("Stack is empty");
        }
    }
}
