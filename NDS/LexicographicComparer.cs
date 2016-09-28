using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Compares two sequences lexicographically.</summary>
    /// <typeparam name="T">The element type of the input sequences.</typeparam>
    public class LexicographicComparer<T> : IComparer<IEnumerable<T>>
    {
        private readonly IComparer<T> comp;

        /// <summary>Creates a new instance with the default comparer for <typeparamref name="T"/>.</summary>
        public LexicographicComparer() : this(Comparer<T>.Default) { }

        /// <summary>Creates a new instance with the given comparer for <typeparamref name="T"/>.</summary>
        /// <param name="comp">Comparer for the items in the input sequences.</param>
        public LexicographicComparer(IComparer<T> comp)
        {
            Contract.Requires(comp != null);
            this.comp = comp;
        }

        /// <summary>
        /// Compares two sequences lexicographically.
        /// Compares each item in the two sequences elementwise. If the items differ according to the comparer
        /// the result of the comparison is the result of the elementwise comparison. If the items are equal
        /// the comparison continues. If one sequence is exhausted before the other it is the smaller sequence
        /// according to this comparer. If the sequences are the same length and contain the same items 0 is
        /// returned.
        /// </summary>
        /// <param name="x">The first sequence.</param>
        /// <param name="y">The second sequence.</param>
        /// <returns>
        /// 0 if <paramref name="x"/> and <paramref name="y"/> are the same length and contain the same items
        /// according to the item comparer. If the result of any comparison between items is non-zero that is
        /// the value returned by this method.
        /// </returns>
        public int Compare(IEnumerable<T> x, IEnumerable<T> y)
        {
            using (var xEnum = x.GetEnumerator())
            using (var yEnum = y.GetEnumerator())
            {
                while(xEnum.MoveNext())
                {
                    if(yEnum.MoveNext())
                    {
                        //compare x.Current and y.Current
                        int c = this.comp.Compare(xEnum.Current, yEnum.Current);

                        //if current element in x and y is equal continue iterating otherwise terminate with result
                        //for current elements
                        if (c != 0) return c;
                    }
                    else
                    {
                        //y is prefix of x so x is larger
                        return 1;
                    }
                }

                //if y has next element then x is a prefix of y and so is smaller
                //if y also has no next element then the sequences are equal elementwise according to comp
                return yEnum.MoveNext() ? -1 : 0;
            }
        }
    }
}
