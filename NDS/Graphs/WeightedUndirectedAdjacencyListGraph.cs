using System;
using System.Collections.Generic;

namespace NDS.Graphs
{
    /// <summary>Undirected adjacency-list representation of a graph with weighted edges.</summary>
    /// <typeparam name="V">Vertex type for the graph.</typeparam>
    /// <typeparam name="W">Weight type for the graph.</typeparam>
    public class WeightedUndirectedAdjacencyListGraph<V, W> : UndirectedAdjacencyListGraphBase<WeightedUndirectedEdge<V, W>, V, W>
    {
        private readonly IEqualityComparer<W> weightComparer;

        public WeightedUndirectedAdjacencyListGraph(IEnumerable<WeightedUndirectedEdge<V, W>> edges, IEqualityComparer<V> vertexComparer, IEqualityComparer<W> weightComparer)
            : base(edges, vertexComparer)
        {
            this.weightComparer = weightComparer;
        }

        protected override WeightedUndirectedEdge<V, W> ReconstructEdge(V sourceVertex, V targetVertex, W weight)
        {
            return new WeightedUndirectedEdge<V, W>(sourceVertex, targetVertex, weight);
        }

        protected override W DeconstructEdge(WeightedUndirectedEdge<V, W> edge)
        {
            return edge.Weight;
        }

        public override IEqualityComparer<WeightedUndirectedEdge<V, W>> EdgeComparer
        {
            get { return new HasWeightEqualityComparer<WeightedUndirectedEdge<V, W>, W>(new UndirectedEdgeEqualityComparer<WeightedUndirectedEdge<V, W>, V>(this.VertexComparer), this.weightComparer); }
        }
    }
}
