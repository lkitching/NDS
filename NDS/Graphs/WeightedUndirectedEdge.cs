namespace NDS.Graphs
{
    /// <summary>Undirected edge with an associated weight.</summary>
    /// <typeparam name="W">Weight type for the edge.</typeparam>
    /// <typeparam name="V">Vertex type for the edge.</typeparam>
    public struct WeightedUndirectedEdge<V, W> : IEdge<V>, IHasWeight<W>
    {
        public WeightedUndirectedEdge(V x, V y, W weight)
            : this()
        {
            this.V1 = x;
            this.V2 = y;
            this.Weight = weight;
        }

        /// <summary>One vertex for the edge.</summary>
        public V V1 { get; private set; }

        /// <summary>The other vertex for the edge.</summary>
        public V V2 { get; private set; }

        /// <summary>Gets the weight for this edge.</summary>
        public W Weight { get; private set; }
    }
}
