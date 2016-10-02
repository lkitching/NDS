using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents a consecutive range [start, end).</summary>
    public struct IntRange : IReadOnlyList<int>, IEquatable<IntRange>
    {
        /// <summary>Creates a range [0, end).</summary>
        /// <param name="end">Exclusive end element of the range.</param>
        public IntRange(int end) : this(0, end) { }

        /// <summary>Creates a range [start, end).</summary>
        /// <param name="start">Inclusive start element of the range.</param>
        /// <param name="end">Exclusive end element of the range.</param>
        public IntRange(int start, int end): this()
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>Gets the start element of this range.</summary>
        public int Start { get; private set; }

        /// <summary>Gets the exclusive end element of this range.</summary>
        public int End { get; private set; }

        /// <summary>Whether this range is empty.</summary>
        public bool IsEmpty
        {
            get { return RangeIsEmpty(this.Start, this.End); }
        }

        /// <summary>Returns whether the range [start, end) is empty.</summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">Exclusive end element of the range.</param>
        /// <returns>Whether the range [start, end) is empty.</returns>
        public static bool RangeIsEmpty(int start, int end)
        {
            return start >= end;
        }

        /// <summary>Find the midpoint of the range [start, end).</summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">Exclusive endpoint of the range.</param>
        /// <returns>The midpoint of [start, end).</returns>
        public static int RangeMidpoint(int start, int end)
        {
            if (RangeIsEmpty(start, end)) return start;
            else
            {
                return start + ((end - 1 - start) / 2);
            }
        }

        /// <summary>Gets the number of elements in the range [start, end).</summary>
        /// <param name="start">Start of the range.</param>
        /// <param name="end">Exclusive end element of the range.</param>
        /// <returns>The number of elements in the range [start, end).</returns>
        public static int RangeCount(int start, int end)
        {
            return Math.Max(0, end - start);
        }

        /// <summary>Returns whether the given value is contained within this range.</summary>
        /// <param name="i">The item to find.</param>
        /// <returns>Whether <paramref name="i"/> exists within this range.</returns>
        public bool Contains(int i)
        {
            return i >= this.Start && i < this.End;
        }

        /// <summary>Gets the number of elements in this range.</summary>
        public int Count
        {
            get { return RangeCount(this.Start, this.End); }
        }

        /// <summary>Gets the item at the given index in this range.</summary>
        /// <param name="idx">The index to fetch./param>
        /// <returns>The item at the given index in this range.</returns>
        public int this[int idx]
        {
            get
            {
                if(idx < 0 || idx >= this.Count)
                {
                    string msg = string.Format("Index must be in range [0, {0})", this.Count);
                    throw new ArgumentOutOfRangeException("idx", idx, msg);
                }
                Contract.EndContractBlock();

                return this.Start + idx;
            }
        }

        /// <summary>Returns a new range [start + count, end).</summary>
        /// <param name="count">The number of items to drop from the start of this range.</param>
        /// <returns>A new range [start + count, end)</returns>
        public IntRange Drop(int count)
        {
            Contract.Requires(count >= 0);
            return new IntRange(this.Start + count, this.End);
        }

        /// <summary>Returns a new range [start, end - count).</summary>
        /// <param name="count">The number of items to keep from the start of this range.</param>
        /// <returns>A new range [start, end - count).</returns>
        public IntRange Take(int count)
        {
            Contract.Requires(count >= 0);
            return (count >= this.Count) ? this : new IntRange(this.Start, this.Start + count);
        }

        /// <summary>Gets an enumerator for the items in this range.</summary>
        /// <returns>An enumerator for the items in this range.</returns>
        public IEnumerator<int> GetEnumerator()
        {
            for(int i = this.Start; i < this.End; ++i)
            {
                yield return i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Whether this range is equal to another. The two ranges are equal if their start and end elements are equal.</summary>
        /// <param name="other">The range to compare to this.</param>
        /// <returns>Whether this range is equal to <paramref name="other"/>.</returns>
        public bool Equals(IntRange other)
        {
            return this.Start == other.Start && this.End == other.End;
        }

        public override bool Equals(object obj)
        {
            return obj is IntRange && this.Equals((IntRange)obj);
        }

        /// <summary>Gets a hashcode for this range.</summary>
        /// <returns>A hashcode for this range.</returns>
        public override int GetHashCode()
        {
            return this.Start + 17 * this.End;
        }
    }
}
