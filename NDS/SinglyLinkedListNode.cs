using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>
    /// Represents a linked list node with a single successor. The value for this node is immutable while the reference
    /// to the next node can be modified using the member functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SinglyLinkedListNode<T>
    {
        /// <summary>Creates a new node with the given value and no successor.</summary>
        /// <param name="value">The value for this node.</param>
        public SinglyLinkedListNode(T value)
            : this(value, null)
        {
        }

        /// <summary>Creates a new node with the given value and successor node.</summary>
        /// <param name="value">The value for this node.</param>
        /// <param name="next">The successor for this node, or null if there is no successor.</param>
        public SinglyLinkedListNode(T value, SinglyLinkedListNode<T> next)
        {
            this.Value = value;
            this.Next = next;
        }

        /// <summary>Gets whether this node has a successor.</summary>
        public bool HasNext
        {
            get { return this.Next != null; }
        }

        /// <summary>
        /// Inserts this node as the successor of the given node. If <paramref name="predecessor"/> has an existing successor it will
        /// be replaced.
        /// </summary>
        /// <param name="predecessor">The node to set as the predecessor of this node.</param>
        public void InsertAfter(SinglyLinkedListNode<T> predecessor)
        {
            Contract.Requires(predecessor != null);
            predecessor.Next = this;
        }

        /// <summary>
        /// Inserts this node between the given nodes. After this operation, the successor of <paramref name="previous"/>
        /// will point to this node, while <paramref name="successor"/> will be the successor of this node.
        /// </summary>
        /// <param name="predecessor">Node to set as the predecessor of this node.</param>
        /// <param name="successor">Node to set as the successor of this node.</param>
        public void InsertBetween(SinglyLinkedListNode<T> predecessor, SinglyLinkedListNode<T> successor)
        {
            Contract.Requires(predecessor != null);

            var pnext = predecessor.Next;
            predecessor.Next = this;
            this.Next = successor;
        }

        /// <summary>
        /// Unlinks the successor node for this node if one exists. After this operation, the successor for this
        /// node will be the successor of the current successor. This can be used to delete nodes from a linked list.
        /// </summary>
        public void UnlinkNext()
        {
            if (this.Next != null)
            {
                this.Next = this.Next.Next;
            }
        }

        /// <summary>Gets a sequence containing this node and all successors.</summary>
        /// <returns>A sequence of this node followed by all successors.</returns>
        public IEnumerable<SinglyLinkedListNode<T>> EnumerateFrom()
        {
            for (var current = this; current != null; current = current.Next)
            {
                yield return current;
            }
        }

        /// <summary>Gets the value for this node.</summary>
        public T Value { get; private set; }

        /// <summary>Gets the successor of this node, or null if there is none.</summary>
        public SinglyLinkedListNode<T> Next { get; private set; }
    }
}
