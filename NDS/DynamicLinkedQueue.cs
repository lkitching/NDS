using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NDS 
{
    /// <summary>Represents a queue implemented as a linked structure which grows dynamically as new items are added.</summary>
    /// <typeparam name="T">The type of items in this queue.</typeparam>
    public class DynamicLinkedQueue<T> : IQueue<T>
    {
        private SinglyLinkedListNode<T> head;
        private SinglyLinkedListNode<T> last;
        private int count;

        /// <see cref="IQueue{T}.Enqueue"/>
        public void Enqueue(T item)
        {
            var newNode = new SinglyLinkedListNode<T>(item);

            if (this.last == null)
            {
                Debug.Assert(this.head == null);
                this.head = newNode;
            }
            else
            {
                Debug.Assert(this.last.Next == null);
                newNode.InsertAfter(this.last);
            }

            this.last = newNode;
            this.count++;
        }

        /// <see cref="IQueue{T}.Dequeue"/>.
        public T Dequeue()
        {
            if (this.Count == 0) throw new InvalidOperationException("Queue is empty");

            Debug.Assert(this.head != null);
            Debug.Assert(this.last != null);

            T removed = this.head.Value;
            this.head = this.head.Next;

            if (this.head == null)
            {
                this.last = null;
            }

            this.count--;
            return removed;
        }

        /// <summary>Gets the number of items in this queue.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Returns true.</summary>
        public bool CanEnqueue
        {
            get { return true; }
        }

        /// <summary>Gets an enumerator for this queue.</summary>
        /// <returns>An enumerator for the items in this queue.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.head.EnumerateFrom().Select(n => n.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
