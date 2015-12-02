using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS.Graphs
{
    /// <summary>Equality comparer for <see cref="UndirectedEdge{V}"/>.</summary>
    /// <typeparam name="V">The type of vertices connected by the edge.</typeparam>
    public class UndirectedEdgeEqualityComparer<V> : IEqualityComparer<UndirectedEdge<V>>
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
        public bool Equals(UndirectedEdge<V> e1, UndirectedEdge<V> e2)
        {
            return (this.vertexComparer.Equals(e1.X, e2.X) && this.vertexComparer.Equals(e1.Y, e2.Y) ||
                   (this.vertexComparer.Equals(e1.X, e2.Y) && this.vertexComparer.Equals(e1.Y, e2.X)));
        }

        /// <summary>Gets a hash code for the given edge based on the hash code of the two vertices.</summary>
        /// <param name="obj">The edge to calculate the hash code for.</param>
        /// <returns>A hash code for <see cref="obj"/>.</returns>
        public int GetHashCode(UndirectedEdge<V> obj)
        {
            return this.vertexComparer.GetHashCode(obj.X) ^ this.vertexComparer.GetHashCode(obj.Y);
        }
    }
}
