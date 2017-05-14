using System;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>
    /// Represents disjoint sets of elements in the range [0, Size). On construction each element in the range
    /// constitutes a singleton set and FindRepresentative(i) == i.
    /// </summary>
    public class IntDisjointSet : IDisjointSet<int>
    {
        private readonly int[] elements;

        /// <summary>
        /// Creates a new set of sets with elements in the range [0, size) where each element constitutes a disjoint
        /// singleton set.
        /// </summary>
        /// <param name="size">The number of elements.</param>
        public IntDisjointSet(int size)
        {
            Contract.Requires(size >= 0);

            this.elements = new int[size];
            for(int i = 0; i < size; ++i)
            {
                this.elements[i] = i;
            }
        }

        /// <see cref="IDisjointSet{T}.FindRepresentative(T)"/> 
        public Maybe<int> FindRepresentative(int item)
        {
            if (this.Contains(item))
            {
                int rep = this.FindExistingRepresentative(item);
                return Maybe.Some(rep);
            }
            else return Maybe.None<int>();
        }

        private int FindExistingRepresentative(int item)
        {
            if (item == this.elements[item]) return item;
            else
            {
                int rep = this.FindExistingRepresentative(this.elements[item]);

                //compress path to representative
                this.elements[item] = rep;
                return rep;
            }
        }

        /// <summary>Merges the two sets containing i and j.</summary>
        /// <param name="i">First item.</param>
        /// <param name="j">Second item.</param>
        /// <exception cref="ArgumentException">If either <paramref name="i"/> or <paramref name="j"/> does not exist.</exception>
        public void Merge(int i, int j)
        {
            if (!this.Contains(i)) throw new ArgumentException("Element does not exist", "i");
            else if (!this.Contains(j)) throw new ArgumentException("Element does not exist", "j");

            int iRep = this.FindExistingRepresentative(i);
            int jRep = this.FindExistingRepresentative(j);

            this.elements[iRep] = jRep;
        }

        /// <summary>Whether the given value is an element of any contained set.</summary>
        /// <param name="item">The item to find.</param>
        /// <returns>Whether <paramref name="item"/> is a member of any contained set.</returns>
        public bool Contains(int item)
        {
            return item >= 0 && item < this.elements.Length;
        }
    }
}
