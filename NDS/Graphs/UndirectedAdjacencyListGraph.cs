using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace NDS.Graphs
{
    /// <summary>
    /// Undirected graph implementation represented as an adjacency list for each vertex in the graph.
    /// </summary>
    /// <typeparam name="V">The type of vertices in this graph.</typeparam>
    public class UndirectedAdjacencyListGraph<V> : UndirectedAdjacencyListGraphBase<UndirectedEdge<V>, V, object>
    {
        /// <summary>Creates a new empty graph with the default equality comparer for the vertex type.</summary>
        public UndirectedAdjacencyListGraph()
            : base(Enumerable.Empty<UndirectedEdge<V>>(), EqualityComparer<V>.Default)
        {
        }

        /// <summary>Creates an empty graph with the given equality comparer for the vertex type.</summary>
        /// <param name="vertexComparer">Comparer for vertices in the graph.</param>
        public UndirectedAdjacencyListGraph(IEqualityComparer<V> vertexComparer)
            : base(Enumerable.Empty<UndirectedEdge<V>>(), vertexComparer)
        {
        }

        /// <summary>Creates a graph containing the given edges and the default comparer for the vertex type.</summary>
        /// <param name="edges">The edges to add to the graph.</param>
        public UndirectedAdjacencyListGraph(IEnumerable<UndirectedEdge<V>> edges)
            : base(edges, EqualityComparer<V>.Default)
        {
        }

        protected override UndirectedEdge<V> ReconstructEdge(V sourceVertex, V targetVertex, object state)
        {
            return new UndirectedEdge<V>(sourceVertex, targetVertex);
        }

        protected override object DeconstructEdge(UndirectedEdge<V> edge)
        {
            return null;
        }

        public override IEqualityComparer<UndirectedEdge<V>> EdgeComparer
        {
            get { return new UndirectedEdgeEqualityComparer<UndirectedEdge<V>, V>(this.VertexComparer); }
        }
    }
}
