using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Collection which is represented as a doubly-linked list which gives efficient access to and insertion at the front and back.</summary>
    /// <typeparam name="T">The type of items in this collection.</typeparam>
    public class DoublyLinkedListCollection<T> : IEnumerable<T>, IRemovable<T>
    {
        private DoublyLinkedListNode<T> first;
        private DoublyLinkedListNode<T> last;
        private int count = 0;

        /// <summary>Adds an item to the front of this collection.</summary>
        /// <param name="value"></param>
        public void AddFirst(T value)
        {
            AddNode(value, n =>
            {
                n.InsertBefore(this.first);
                this.first = n;
            });
        }

        /// <summary>Adds an item to the end of this collection.</summary>
        /// <param name="value">The item to add.</param>
        public void AddLast(T value)
        {
            AddNode(value, n =>
            {
                n.InsertAfter(this.last);
                this.last = n;
            });
        }

        /// <summary>Gets the first item in this collection.</summary>
        /// <exception cref="InvalidOperationException">If this list is empty.</exception>
        public T First
        {
            get
            {
                this.GuardNotEmpty();
                return this.first.Value;
            }
        }

        /// <summary>Gets the last item in this collection.</summary>
        /// <exception cref="InvalidOperationException">If this list is empty.</exception>
        public T Last
        {
            get
            {
                this.GuardNotEmpty();
                return this.last.Value;
            }
        }

        /// <summary>Removes the last item in this collection.</summary>
        /// <returns>The removed item.</returns>
        /// <exception cref="InvalidOperationException">If this list is empty.</exception>
        public T RemoveFirst()
        {
            return this.RemoveEndNode(this.first);
        }

        /// <summary>Removes the last item in this collection.</summary>
        /// <returns>The removed item.</returns>
        /// <exception cref="InvalidOperationException">If this list is empty.</exception>
        public T RemoveLast()
        {
            return this.RemoveEndNode(this.last);
        }

        /// <summary>Removes the first item in this collection matching the given predicate.</summary>
        /// <param name="predicate">The predicate to match items in this collection.</param>
        /// <returns>Whether an item matching <paramref name="predicate"/> was found.</returns>
        public bool RemoveFirstWhere(Func<T, bool> predicate)
        {
            Contract.Requires(predicate != null);

            for (var current = this.first; current != null; current = current.Next)
            {
                if (predicate(current.Value))
                {
                    RemoveNode(current);
                    return true;
                }
            }
            return false;
        }

        /// <summary>Removes all the items in this collection matching the given predicate.</summary>
        /// <param name="predicate">The predicate to match items in this collection.</param>
        /// <returns>The number of items removed.</returns>
        public int RemoveAllWhere(Func<T, bool> predicate)
        {
            Contract.Requires(predicate != null);

            int removed = 0;
            for (var current = this.first; current != null; current = current.Next)
            {
                if (predicate(current.Value))
                {
                    this.RemoveNode(current);
                    removed++;
                }
            }
            return removed;
        }

        /// <summary>The number of items in this collection.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Gets an enumerator for the items in this collection.</summary>
        /// <returns>An enumerator for this collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var current = this.first; current != null; current = current.Next)
            {
                yield return current.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Enumerates the items in this list in reverse.</summary>
        /// <returns>A reverse enumerator for the items in this list.</returns>
        public IEnumerable<T> EnumerateBack()
        {
            return this.last.EnumerateBackFrom().Select(n => n.Value);
        }

        private void RemoveNode(DoublyLinkedListNode<T> node)
        {
            if (object.ReferenceEquals(this.first, node))
            {
                this.first = node.Next;
            }

            if (object.ReferenceEquals(this.last, node))
            {
                this.last = node.Previous;
            }

            node.Unlink();
            this.count--;
        }

        private T GetEndValue(DoublyLinkedListNode<T> endNode)
        {
            this.GuardNotEmpty();
            return endNode.Value;
        }

        private T RemoveEndNode(DoublyLinkedListNode<T> node)
        {
            this.GuardNotEmpty();
            this.RemoveNode(node);
            return node.Value;
        }

        private void GuardNotEmpty()
        {
            if (this.count == 0)
            {
                throw new InvalidOperationException("List is empty");
            }
        }

        private void AddNode(T value, Action<DoublyLinkedListNode<T>> attachAction)
        {
            var newNode = new DoublyLinkedListNode<T>(value);
            if (this.count == 0)
            {
                Debug.Assert(this.first == null);
                Debug.Assert(this.last == null);
                this.first = this.last = newNode;
            }
            else
            {
                Debug.Assert(this.first != null);
                Debug.Assert(this.last != null);
                attachAction(newNode);
            }

            this.count++;
        }
    }
}
