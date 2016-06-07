using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>
    /// Resizable buffer which allows insert and delete operations around a 'point' which can be moved around within the range [0, Count].
    /// The buffer maintains a single buffer with a gap starting after the point which is filled as new elements are inserted. Inserting elements
    /// into the gap is achieved in amortised constant time. Moving the point will cause a block of elements to be moved to restructure the gap
    /// upon the following insert or delete operation.
    /// </summary>
    /// <typeparam name="T">The type of items in the buffer.</typeparam>
    public class GapBuffer<T> : IEnumerable<T>, IInsertable<T>
    {
        private T[] buf;
        private int gapStart;
        private int gapEnd;
        private int point;

        /// <summary>Creates a new empty buffer with the default capacity.</summary>
        public GapBuffer() : this(10) { }

        /// <summary>Creates a new empty buffer with the given capacity. </summary>
        /// <param name="capacity">The initial capcity for the internal buffer.</param>
        public GapBuffer(int capacity)
        {
            Contract.Requires(capacity >= 0);
            this.buf = new T[capacity];
            this.gapStart = 0;
            this.gapEnd = this.buf.Length;
            this.point = 0;
        }

        /// <summary>Inserts the given item at the current point.</summary>
        /// <param name="item">The item to insert.</param>
        public void Insert(T item)
        {
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) + 1);
            Contract.Ensures(this.Point == Contract.OldValue(this.Point) + 1);

            //resize buffer if full
            if(this.Count == this.buf.Length)
            {
                this.Resize();
            }

            //position start of gap at point
            MoveGapToPoint();

            //insert at point
            this.buf[this.point] = item;
            this.point++;
            this.gapStart++;
        }

        private void MoveGapToPoint()
        {
            if (this.point != this.gapStart)
            {
                if (this.point < this.gapStart)
                {
                    //move items between point and start of gap to the beginning of the segment after the gap
                    int moveCount = this.gapStart - this.point;
                    int destEndIndex = this.gapEnd;
                    int destStartIndex = destEndIndex - moveCount;

                    Array.Copy(this.buf, this.point, this.buf, destStartIndex, moveCount);

                    //adjust position of gap
                    this.gapStart = this.point;
                    this.gapEnd -= moveCount;

                    //the items between point and the old start of the gap are junk
                    //EXCEPT any which have been written in front of the old gap end.
                    //find number of invalid indexes and clear them to avoid keeping references alive
                    int invalidCount = Math.Min(moveCount, this.GapLength);
                    Array.Clear(this.buf, this.gapStart, invalidCount);
                }
                else if (this.point > this.gapEnd)
                {
                    //move items between end of gap and point to the end of the segment after the start of the gap
                    int moveCount = this.point - this.gapEnd;
                    Array.Copy(this.buf, this.gapEnd, this.buf, this.gapStart, moveCount);

                    //adjust position of gap
                    this.gapStart += moveCount;
                    this.gapEnd += moveCount;
                    this.point = this.gapStart;

                    //the items between the old gap end and old point are junk
                    //EXCEPT any which have been written to the start of the old gap
                    //find the number of invalid indexes and remove them
                    int invalidCount = Math.Min(moveCount, this.GapLength);
                    Array.Clear(this.buf, this.gapEnd - invalidCount, invalidCount);
                }
                else
                {
                    //should never happen!
                    Debug.Assert(this.gapStart > this.gapEnd, "Gap start index > Gap end index");
                    Debug.Fail("Failed invariant: gapStart > gapEnd");
                }
            }

            Debug.Assert(this.point == this.gapStart);
        }

        private int GapLength
        {
            get { return this.gapEnd - this.gapStart; }
        }

        private void Resize()
        {
            var newBuf = new T[this.buf.Length * 2];

            //copy everything before the point into the new section before the gap
            if(this.point > 0)
            {
                Array.Copy(this.buf, 0, newBuf, 0, this.point);
            }

            //copy everything at the end of the gap to the end of the new buffer
            int endCount = this.buf.Length - this.gapEnd;
            int newGapEnd = newBuf.Length - endCount;

            if (endCount > 0)
            {
                Array.Copy(this.buf, this.gapEnd, newBuf, newGapEnd, endCount);
            }

            //copy everything between point and start of the current gap to the start of the section after the gap
            int afterPointCount = this.gapStart - this.point;
            
            if (afterPointCount > 0)
            {
                newGapEnd -= afterPointCount;
                Array.Copy(this.buf, this.point, newBuf, newGapEnd, afterPointCount);
            }

            //adjust gap location
            this.gapStart = this.point;
            this.gapEnd = newGapEnd;

            this.buf = newBuf;
        }

        /// <summary>Removes the element immediately following the point.</summary>
        /// <returns>The removed item.</returns>
        public T RemoveNext()
        {
            Contract.Requires(this.Point < this.Count);
            Contract.Ensures(this.Point == Contract.OldValue(this.Point));
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) - 1);

            if (this.Count == 0 || this.point == this.Count) throw new InvalidOperationException("No next element to remove");
            this.MoveGapToPoint();
            Debug.Assert(this.gapEnd < this.buf.Length);

            var removing = this.buf[this.gapEnd];
            this.buf[this.gapEnd] = default(T);

            this.gapEnd++;
            return removing;
        }

        /// <summary>Removes the item immediately preceding the point.</summary>
        /// <returns>The removed item.</returns>
        public T RemovePrevious()
        {
            Contract.Requires(this.Count > 0);
            Contract.Ensures(this.Point == Contract.OldValue(this.Point) - 1);
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) - 1);

            if (this.Count == 0 || this.point == 0) throw new InvalidOperationException("No previous element to remove");
            this.MoveGapToPoint();

            //NOTE: point now contains of first free index in the gap
            //move it back first to point to removing index
            this.point--;

            //remove reference to removing value
            var removing = this.buf[this.point];
            this.buf[this.point] = default(T);

            this.gapStart--;
            
            return removing;
        }

        /// <summary>Gets and sets the current location of the point. This should always be a value in the range [0, Count].</summary>
        public int Point
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= this.Count);

                if (this.point <= this.gapStart) return this.point;
                else
                {
                    int endOffset = this.point - this.gapEnd;
                    return this.gapStart + endOffset + 1;
                }
            }
            set
            {
                if (value < 0 || value > this.Count)
                {
                    var msg = string.Format("Point must be in the range [0, {0}]: Received {1}", this.Count, value);
                    throw new ArgumentOutOfRangeException(msg);
                }
                Contract.EndContractBlock();

                if (value <= this.gapStart)
                {
                    this.point = value;
                }
                else
                {
                    int diff = value - this.point;
                    this.point = this.gapEnd + diff - 1;
                }
            }
        }

        /// <summary>Gets the number of items in this buffer.</summary>
        public int Count
        {
            get { return this.buf.Length - (this.gapEnd - this.gapStart); }
        }

        /// <summary>Gets the capacity of the internal buffer including the gap.</summary>
        public int Capacity
        {
            get { return this.buf.Length; }
        }

        /// <summary>Returns the sequence of items in this buffer.</summary>
        /// <returns>An enumerator for the items in this buffer.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < this.gapStart; ++i)
            {
                yield return this.buf[i];
            }

            for(int i = this.gapEnd; i < this.buf.Length; ++i)
            {
                yield return this.buf[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
