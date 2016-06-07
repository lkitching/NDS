using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NDS 
{
    public class ArrayList<T> : IEnumerable<T>, IRemovable<T>, IInsertable<T>
    {
        private static readonly int DEFAULT_CAPACITY = 10;

        private T[] items;
        private int count;

        public ArrayList()
            : this(DEFAULT_CAPACITY)
        {
        }

        public ArrayList(IEnumerable<T> items)
            : this(items, DEFAULT_CAPACITY)
        {
        }

        public ArrayList(int capacity)
        {
            if (capacity < 0) throw new ArgumentOutOfRangeException("capacity must be non-negative");

            this.items = new T[capacity];
            this.count = 0;
        }

        public ArrayList(IEnumerable<T> items, int capacity)
        {
            T[] initial = items.EmptyIfNull().ToArray();

            if (capacity > initial.Length)
            {
                this.items = new T[capacity];
                initial.CopyTo(this.items, 0);
            }
            else
            {
                this.items = initial;
            }

            this.count = initial.Length;
        }

        public void Insert(T item)
        {
            this.Add(item);
        }

        public void Add(T item)
        {
            this.Add(item, this.count);
        }

        public void Add(T item, int index)
        {
            int addIndex = index == this.count ? index : this.CalculateEffectiveIndex(index);

            this.EnsureCapacity(this.count + 1);

            //make room for new item
            //start at end of items and shift each element to the right, moving left
            for (int i = this.count - 1; i >= addIndex; i--)
            {
                this.items[i + 1] = this.items[i];
            }

            this.items[addIndex] = item;
            this.count++;
        }

        private void EnsureCapacity(int minSize)
        {
            //should only occur due to overflow
            if (minSize < 0) throw new OutOfMemoryException("Overflow in size of ArrayList");

            Debug.Assert(minSize > this.count);
            if (this.items.Length >= minSize) return;

            int newCapacity = minSize * 2;
            
            //check overflow
            if (newCapacity < 0) newCapacity = int.MaxValue;

            //ensure capacity is at least the default size
            if (newCapacity < DEFAULT_CAPACITY) newCapacity = DEFAULT_CAPACITY;

            Debug.Assert(newCapacity > this.count);
            Debug.Assert(newCapacity >= DEFAULT_CAPACITY);

            //create new arary and copy old items
            T[] newItems = new T[newCapacity];
            Array.Copy(this.items, newItems, this.count);

            this.items = newItems;
        }

        public bool Contains(T item)
        {
            return this.Contains(item, EqualityComparer<T>.Default);
        }

        public bool Contains(T item, IEqualityComparer<T> comparer)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;

            for (int i = 0; i < this.count; i++)
            {
                if (comparer.Equals(item, this.items[i])) return true;
            }

            return false;
        }

        public bool RemoveFirstWhere(Func<T, bool> pred)
        {
            bool match = false;
            int i = 0;

            //find index of first match
            for (; i < this.count; i++)
            {
                if (pred(this.items[i]))
                {
                    match = true;
                    break;
                }
            }

            if (!match) return false;

            //shift all items at indices i+1...count-1 to the left
            for (int j = i + 1; j < this.count; j++)
            {
                this.items[j - 1] = this.items[j];
            }

            //item at index count-1 is no longer valid
            this.items[this.count - 1] = default(T);

            this.count--;
            return true;
        }

        public int RemoveAllWhere(Func<T, bool> pred)
        {
            if (pred == null) throw new ArgumentNullException("pred");

            int removed = 0;
            for (int i = 0; i < this.count; i++)
            {
                T currentItem = this.items[i];
                if (pred(currentItem))
                {
                    removed++;
                }
                else if (removed > 0)
                {
                    //shift current element down to new position
                    this.items[i - removed] = currentItem;
                }
            }

            int newCount = this.count - removed;
            Debug.Assert(newCount >= 0);

            //remove any references to removed items
            //items at indices [0, newCount-1] are still in the list
            //items at [newCount, oldCount-1] are no longer valid
            for (int i = newCount; i < this.count; i++)
            {
                this.items[i] = default(T);
            }

            this.count = newCount;
            return removed;
        }

        public T RemoveAt(int index)
        {
            int effectiveIndex = CalculateEffectiveIndex(index);
            T toRemove = this.items[effectiveIndex];

            this.RemoveAt(new int[] { effectiveIndex });
            return toRemove;
        }

        private void RemoveAt(IEnumerable<int> indices)
        {
            Debug.Assert(indices != null);

            int holes = 0;
            using(var iter = indices.GetEnumerator())
            {
                if(! iter.MoveNext()) return;

                int currentIndex = iter.Current + 1;
                holes = 1;

                while (iter.MoveNext())
                {
                    int nextIndex = iter.Current;
                    this.ShiftDown(currentIndex, nextIndex, holes);
                    holes++;
                }

                //shift remaining elements to the end of the list
                this.ShiftDown(currentIndex, this.count, holes);
            }

            Debug.Assert(holes > 0);

            //all 'holes' in the list have now been moved to the end
            int newCount = this.count - holes;
            Debug.Assert(newCount >= 0);

            //remove any references to removed items
            for (int i = newCount; i < this.count; i++)
            {
                this.items[i] = default(T);
            }

            this.count = newCount;
        }

        private void ShiftDown(int startIndex, int endIndex, int distance)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                this.items[i - distance] = this.items[i];
            }
        }

        public void Clear()
        {
            Array.Clear(this.items, 0, this.items.Length);
            this.count = 0;
        }

        public void Trim()
        {
            if (this.count > this.items.Length)
            {
                T[] newItems = new T[this.count];
                for (int i = 0; i < newItems.Length; i++)
                {
                    newItems[i] = this.items[i];
                }

                this.items = newItems;
            }
        }

        public T this[int index]
        {
            get
            {
                int effectiveIndex = this.CalculateEffectiveIndex(index);
                return this.items[effectiveIndex];
            }
            set
            {
                int effectiveIndex = this.CalculateEffectiveIndex(index);
                this.items[effectiveIndex] = value;
            }
        }

        public int Count
        {
            get { return this.count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.count; i++)
            {
                yield return this.items[i];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private int CalculateEffectiveIndex(int idx)
        {
            if (idx >= this.count || idx < (-this.count)) 
                throw new ArgumentOutOfRangeException(string.Format("Index must be in the range [{0}, {1}", -this.count, this.count - 1));

            return idx >= 0 ? idx : idx + this.count;
        }
    }
}
