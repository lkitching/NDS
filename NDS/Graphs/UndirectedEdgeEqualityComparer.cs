using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS.Graphs
{
    /// <summary>Equality comparer for <see cref="UndirectedEdge{V}"/>.</summary>
    /// <typeparam name="V">The type of vertices connected by the edge.</typeparam>
    public class UndirectedEdgeEqualityComparer<E, V> : IEqualityComparer<E>
        where E : IEdge<V>
    {
        private readonly IEqualityComparer<V> vertexComparer;

        /// <summary>Creates a new comparer with the default equality comparer for the vertex type.</summary>
        public UndirectedEdgeEqualityComparer()
            : this(EqualityComparer<V>.Default)
        {
        }

        /// <summary>Creates a new comparer with the given equality comparer for the vertex type.</summary>
        /// <param name="vertexComparer">Equality comparer for the vertex type.</param>
        public UndirectedEdgeEqualityComparer(IEqualityComparer<V> vertexComparer)
        {
            Contract.Requires(vertexComparer != null);
            this.vertexComparer = vertexComparer;
        }

        /// <summary>
        /// Compares two undirected edges for equality. Two edges are equal if they are oriented in the same or opposite direction
        /// and their corresponding vertices are equal. This means the edge (x, y) is equal to both (x', y') and (y', x') as long as
        /// x == x' and y == y'.
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public bool Equals(E e1, E e2)
        {
            return (this.vertexComparer.Equals(e1.V1, e2.V1) && this.vertexComparer.Equals(e1.V2, e2.V2) ||
                   (this.vertexComparer.Equals(e1.V1, e2.V2) && this.vertexComparer.Equals(e1.V2, e2.V1)));
        }

        /// <summary>Gets a hash code for the given edge based on the hash code of the two vertices.</summary>
        /// <param name="obj">The edge to calculate the hash code for.</param>
        /// <returns>A hash code for <see cref="obj"/>.</returns>
        public int GetHashCode(E obj)
        {
            return this.vertexComparer.GetHashCode(obj.V1) ^ this.vertexComparer.GetHashCode(obj.V2);
        }
    }
}
