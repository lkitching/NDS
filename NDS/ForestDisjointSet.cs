using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Disjoint forest representation of a disjoint set.</summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    public class ForestDisjointSet<T> : IDisjointSet<T>
    {
        private readonly IEqualityComparer<T> comparer;
        private readonly ClosedAddressingHashTable<T, ForestDisjointSetTreeNode<T>> nodes;

        /// <summary>Creates a new empty collection with the default equality comparer for <typeparamref name="T"/>.</summary>
        public ForestDisjointSet() : this(EqualityComparer<T>.Default) { }

        /// <summary>Creates a new empty collection with the given equality comparer.</summary>
        /// <param name="comparer">Equality comparer for elements in the set.</param>
        public ForestDisjointSet(IEqualityComparer<T> comparer)
        {
            Contract.Requires(comparer != null);
            this.comparer = comparer;
            this.nodes = new ClosedAddressingHashTable<T, ForestDisjointSetTreeNode<T>>(comparer);
        }

        /// <summary>
        /// Creates a new singleton set containing the given element. If a singleton set already exists for <paramref name="item"/> no changes are made. 
        /// If a non-singelton set exists an exception is thrown.</summary>
        /// <param name="item">The item to create a singleton set for.</param>
        /// <exception cref="ArgumentException">If a non-singleton set already exists for <paramref name="item"/>.</exception>
        public void MakeSet(T item)
        {
            var maybeNode = this.nodes.Get(item);
            if (maybeNode.HasValue)
            {
                if (!maybeNode.Value.IsRoot)
                {
                    throw new ArgumentException("Non-singleton set already exists for item");
                }
            }
            else
            {
                this.nodes.Add(item, new ForestDisjointSetTreeNode<T>(item));
            }
        }

        /// <see cref="IDisjointSet{T}.FindRepresentative(T)"/> 
        public Maybe<T> FindRepresentative(T item)
        {
            ForestDisjointSetTreeNode<T> representativeNode = FindRepresentativeNode(item);
            return representativeNode == null ? Maybe.None<T>() : Maybe.Some(representativeNode.Value);
        }

        private ForestDisjointSetTreeNode<T> FindRepresentativeNode(T item)
        {
            var maybeNode = this.nodes.Get(item);
            if (maybeNode.HasValue)
            {
                var node = maybeNode.Value;
                var searchPath = new ArrayList<ForestDisjointSetTreeNode<T>>();
                while (!node.IsRoot)
                {
                    searchPath.Add(node);
                    node = node.Parent;
                }

                //update all nodes on the search path to point directly to the parent
                foreach (var pathNode in searchPath)
                {
                    pathNode.Parent = node;
                }

                return node;
            }
            else return null;
        }

        private ForestDisjointSetTreeNode<T> FindRequiredRepresentativeNode(T item)
        {
            var repNode = FindRepresentativeNode(item);
            if (repNode == null) throw new ArgumentException(item + " not found in set");
            else return repNode;
        }

        /// <see cref="IDisjointSet{T}.Merge(T, T)"/> 
        public void Merge(T x, T y)
        {
            var xRepNode = FindRequiredRepresentativeNode(x);
            var yRepNode = FindRequiredRepresentativeNode(y);

            ForestDisjointSetTreeNode.MergeTrees(xRepNode, yRepNode);
        }
    }
}
