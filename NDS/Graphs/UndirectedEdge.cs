namespace NDS.Graphs
{
    /// <summary>Represents an undirected edge between two vertices.</summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    public struct UndirectedEdge<V>
    {
        /// <summary>Creates an edge between the given vertices.</summary>
        /// <param name="x">The first vertex.</param>
        /// <param name="y">The second vertex.</param>
        public UndirectedEdge(V x, V y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>One vertex in the edge.</summary>
        public V X { get; private set; }

        /// <summary>The other vertex in the edge.</summary>
        public V Y { get; private set; }
    }
}
