using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>
    /// Singly-linked list which allows access to the nodes it contains. Since these can be freely manipulated, the list cannot track
    /// the count of the number of elements it contains. This is in contrast to <see cref="SinglyLinkedListCollection{T}"/> which does
    /// not allow access to its nodes and tracks the number of contained elements.
    /// </summary>
    /// <typeparam name="T">Type of values in the nodes in this list.</typeparam>
    public class SinglyLinkedList<T> : IEnumerable<SinglyLinkedListNode<T>>
    {
        /// <summary>Gets the head of this list if one exists. Returns null if this list is empty.</summary>
        public SinglyLinkedListNode<T> First { get; private set; }

        /// <summary>Adds a given node to the front of this list.</summary>
        /// <param name="node">The node to add.</param>
        public void AddFirst(SinglyLinkedListNode<T> node)
        {
            Contract.Requires(node != null);

            if (this.First != null)
            {
                this.First.InsertAfter(node);
            }

            this.First = node;
        }

        /// <summary>Adds a node containing the given value to the front of this list.</summary>
        /// <param name="value">The value to add.</param>
        public void AddFirst(T value)
        {
            this.AddFirst(new SinglyLinkedListNode<T>(value));
        }

        /// <summary>Removes the first node in this list if one exists.</summary>
        public SinglyLinkedListNode<T> RemoveFirst()
        {
            if (this.First == null) throw new InvalidOperationException("List empty");

            var removed = this.First;
            this.First = removed.Next;
            return removed;
        }

        /// <summary>Gets whether this list is empty.</summary>
        public bool IsEmpty
        {
            get { return this.First == null; }
        }

        /// <summary>Gets an enumerator for the nodes in this list.</summary>
        /// <returns>An enumerator for the nodes in this list.</returns>
        public IEnumerator<SinglyLinkedListNode<T>> GetEnumerator()
        {
            var e = this.First == null ? Enumerable.Empty<SinglyLinkedListNode<T>>() : this.First.EnumerateFrom();
            return e.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
