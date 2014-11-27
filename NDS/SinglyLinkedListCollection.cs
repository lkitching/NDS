using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>
    /// Represents a singly-linked list of items. No access is given to the internal structure of the list so the collection
    /// tracks the number of contained items.
    /// </summary>
    /// <typeparam name="T">The type of values in the collection.</typeparam>
    public class SinglyLinkedListCollection<T> : IEnumerable<T>, IRemovable<T>
    {
        private SinglyLinkedListNode<T> first;
        private int count = 0;

        /// <summary>Creates a new list containing the given items. The order of items will be maintained in the created list.</summary>
        /// <param name="items">The sequence to initialise this list with.</param>
        public SinglyLinkedListCollection(IEnumerable<T> items)
        {
            Contract.Requires(items != null);
            InitialiseFrom(items.ToArray());
        }

        /// <summary>Creates a new list containing the given items. The order of items will be maintained in the created list.</summary>
        /// <param name="items">The collection to intialise this list with.</param>
        public SinglyLinkedListCollection(params T[] items)
        {
            this.InitialiseFrom(items);
        }

        private void InitialiseFrom(T[] items)
        {
            for (int i = items.Length - 1; i >= 0; --i)
            {
                this.AddFirst(items[i]);
            }
        }

        /// <summary>Adds the given item to the front of this collection.</summary>
        /// <param name="item">The item to add.</param>
        public void AddFirst(T item)
        {
            if (this.first == null)
            {
                this.first = new SinglyLinkedListNode<T>(item);
            }
            else
            {
                this.first = new SinglyLinkedListNode<T>(item, this.first);
            }

            this.count++;
        }

        /// <summary>Removes the first element in this list if one exists.</summary>
        /// <returns>The removed item.</returns>
        /// <exception cref="InvalidOperationException">If this list is empty.</exception>
        public T RemoveFirst()
        {
            if (this.count == 0) throw new InvalidOperationException("Empty list");

            Debug.Assert(this.first != null);
            var oldFirst = this.first;
            this.first = oldFirst.Next;
            this.count--;
            return oldFirst.Value;
        }

        /// <summary>Removes the first item in this list for which the given predicate returns true.</summary>
        /// <param name="predicate">The predicate to test items against.</param>
        /// <returns>True if a matching item was found.</returns>
        public bool RemoveFirstWhere(Func<T, bool> predicate)
        {
            if (this.count == 0) return false;

            Debug.Assert(this.first != null);

            //remove head if it fails the predicate
            if (predicate(this.first.Value))
            {
                this.first = this.first.Next;
                this.count--;
                return true;
            }

            var prev = this.first;
            var current = this.first.Next;

            while (current != null)
            {
                if (predicate(current.Value))
                {
                    prev.UnlinkNext();
                    this.count--;
                    return true;
                }

                prev = current;
                current = current.Next;
            }

            return false;
        }

        /// <summary>Removes all items from this list for with <paramref name="predicate"/> return true.</summary>
        /// <param name="predicate">The predicate to match items to be removed.</param>
        /// <returns>The number of removed items.</returns>
        public int RemoveAllWhere(Func<T, bool> predicate)
        {
            Contract.Requires(predicate != null);

            if (this.Count == 0) return 0;

            int removed = 0;

            //keep removing the head while it fails the predicate
            while (this.first != null && predicate(this.first.Value))
            {
                this.first = this.first.Next;
                removed++;
            }

            if (this.first == null)
            {
                //removed all elements
                Debug.Assert(removed == this.count);
                this.count -= removed;
                return removed;
            }

            //walk through the remaining list by keeping track of its predecessor
            //Unlink the current node every time its value fails the predicate
            //Move to the next node if the value of the next node passes
            var pred = this.first;

            while (pred != null && pred.Next != null)
            {
                while (pred.Next != null && predicate(pred.Next.Value))
                {
                    pred.UnlinkNext();
                    removed++;
                }

                pred = pred.Next;
            }

            this.count -= removed;
            return removed;
        }

        /// <summary>Gets the number of elements in this list.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Gets the first item in this list if one exists.</summary>
        /// <exception cref="InvalidOperationException">If this list is empty.</exception>
        public T First
        {
            get
            {
                if (this.first == null) throw new InvalidOperationException("List empty");
                Debug.Assert(this.count != 0);
                return this.first.Value;
            }
        }

        /// <summary>Gets an enumerator for the items in this list.</summary>
        /// <returns>An enumerator for the items in this list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.first.EnumerateFrom().Select(n => n.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
