using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents a linked list node with a predecessor and successor.</summary>
    /// <typeparam name="T">The value of this node.</typeparam>
    public class DoublyLinkedListNode<T>
    {
        /// <summary>Creates a new node with the given value and no predecessor or successor.</summary>
        /// <param name="value">The value for this node.</param>
        public DoublyLinkedListNode(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Inserts this node before the given node. If the predecessor to this node is not already set, it will be set to the 
        /// predecessor of <paramref name="next"/>.
        /// </summary>
        /// <param name="next"></param>
        public void InsertBefore(DoublyLinkedListNode<T> next)
        {
            Contract.Requires(next != null);

            if (this.Previous == null)
            {
                this.Previous = next.Previous;
            }

            next.Previous = this;
            this.Next = next;

        }

        /// <summary>
        /// Inserts this node after the given node. If the successor to this node is not already set, it will be set to the
        /// successor of <paramref name="previous"/>.
        /// </summary>
        /// <param name="previous"></param>
        public void InsertAfter(DoublyLinkedListNode<T> previous)
        {
            Contract.Requires(previous != null);

            if (this.Next == null)
            {
                this.Next = previous.Next;
            }

            previous.Next = this;
            this.Previous = previous;
        }

        /// <summary>
        /// Unlinks this node from its predecessor and successor and points them to each other. If Previous and Next for
        /// this node are non-null before this operation then after it, the successor of the predecessor will be this
        /// node's successor and vice versa.
        /// </summary>
        public void Unlink()
        {
            if (this.Previous != null)
            {
                this.Previous.Next = this.Next;
            }

            if (this.Next != null)
            {
                this.Next.Previous = this.Previous;
            }
        }

        /// <summary>Gets the value for this node.</summary>
        public T Value { get; private set; }
        
        /// <summary>Gets the successor for this node if one exists.</summary>
        public DoublyLinkedListNode<T> Next { get; private set; }

        /// <summary>Gets the predecessor for this node if one exists.</summary>
        public DoublyLinkedListNode<T> Previous { get; private set; }
    }
}
