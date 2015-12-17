using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    internal class ForestDisjointSetTreeNode<T>
    {
        internal ForestDisjointSetTreeNode(T value)
        {
            this.Value = value;
            this.Rank = 0;
            this.Parent = this;
        }

        public T Value { get; private set; }
        public ForestDisjointSetTreeNode<T> Parent { get; set; }
        public int Rank { get; set; }

        public bool IsRoot
        {
            get { return object.ReferenceEquals(this, this.Parent); }
        }
    }

    internal class ForestDisjointSetTreeNode
    {
        internal static void MergeTrees<T>(ForestDisjointSetTreeNode<T> x, ForestDisjointSetTreeNode<T> y)
        {
            Contract.Requires(x != null);
            Contract.Requires(y != null);
            Contract.Requires(x.IsRoot);
            Contract.Requires(y.IsRoot);

            //nothing to do if x and y already have the same representative
            if (object.ReferenceEquals(x, y)) return;

            //make the smaller-ranked tree the child of the lager one
            //if both have the same rank then arbitrarily make xRepNode the new parent
            if (x.Rank < y.Rank)
            {
                x.Parent = y;
            }
            else if (y.Rank < x.Rank)
            {
                y.Parent = x;
            }
            else
            {
                Debug.Assert(x.Rank == y.Rank);
                y.Parent = x;
                x.Rank += 1;
            }
        }
    }
}
