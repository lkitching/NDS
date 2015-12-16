using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NDS.Graphs
{
    public abstract class UndirectedAdjacencyListGraphBase<E, V, VState> : IGraph<E, V>
        where E : IEdge<V>
    {
        private readonly IEqualityComparer<V> vertexComparer;
        private readonly Dictionary<V, HashSet<VertexState>> edges;

        private struct VertexState
        {
            public VertexState(V vertex, VState state)
                : this()
            {
                this.Vertex = vertex;
                this.State = state;
            }

            public VState State { get; private set; }
            public V Vertex { get; private set; }
        }

        /// <summary>Creates a graph containing the given edges and comparer for the vertex type.</summary>
        /// <param name="edges">The edges to add to the graph.</param>
        /// <param name="vertexComparer">The equality comparer to use for vertices.</param>
        public UndirectedAdjacencyListGraphBase(IEnumerable<E> edges, IEqualityComparer<V> vertexComparer)
        {
            this.vertexComparer = vertexComparer;

            this.edges = new Dictionary<V, HashSet<VertexState>>(vertexComparer);

            foreach (var edge in edges)
            {
                this.AddEdge(edge);
            }
        }

        /// <summary>Adds the given edge to this graph.</summary>
        /// <param name="edge">The edge to add.</param>
        public void AddEdge(E edge)
        {
            AddDirected(edge.V1, edge.V2, edge);
            AddDirected(edge.V2, edge.V1, edge);
        }

        /// <summary>Removes the given edge from this graph if it exists.</summary>
        /// <param name="edge"></param>
        public void RemoveEdge(E edge)
        {
            this.RemoveAdjacency(edge.V1, edge.V2);
            this.RemoveAdjacency(edge.V2, edge.V1);
        }

        /// <summary>Removes the given vertex and all incident edges from this graph.</summary>
        /// <param name="vertex">The vertex to remove.</param>
        /// <returns>True if the vertex existed in this graph, otherwise false.</returns>
        public bool RemoveVertex(V vertex)
        {
            HashSet<VertexState> adj;
            if (this.edges.TryGetValue(vertex, out adj))
            {
                this.edges.Remove(vertex);
                foreach (VertexState target in adj)
                {
                    this.RemoveAdjacency(target.Vertex, vertex);
                }
                return true;
            }
            else return false;
        }

        public bool ContainsVertex(V vertex)
        {
            return this.edges.ContainsKey(vertex);
        }

        private void RemoveAdjacency(V source, V target)
        {
            var adj = this.GetAdjacencySetFor(source);

            //NOTE: state is not used for equality so don't need it to remove the vertex
            //from the adjacency set
            VertexState state = new VertexState(target, default(VState));
            adj.Remove(state);
            if (adj.Count == 0)
            {
                bool removedAdj = this.edges.Remove(source);
                Debug.Assert(removedAdj, "Failed to remove vertex with empty adjacency set");
            }
        }

        /// <summary>Gets all the vertices adjacent to the given vertex.</summary>
        /// <param name="vertex">The vertex to find adjacent vertices for.</param>
        /// <returns>All the vertices connected to <paramref name="vertex"/> by an edge in this graph.</returns>
        public IEnumerable<V> GetAdjacentVertices(V vertex)
        {
            return this.edges.GetOrValue(vertex, null).EmptyIfNull().Select(s => s.Vertex);
        }

        /// <summary>Finds all edges incident on a given vertex.</summary>
        /// <param name="vertex">The vertex to find incident edges for.</param>
        /// <returns>All edges incident on the given vertex.</returns>
        public IEnumerable<E> GetAdjacentEdges(V vertex)
        {
            var adjacentVertices = GetAdjacencySetFor(vertex);
            foreach (VertexState adj in adjacentVertices)
            {
                yield return ReconstructEdge(vertex, adj.Vertex, adj.State);
            }
        }

        protected abstract E ReconstructEdge(V sourceVertex, V targetVertex, VState state);
        protected abstract VState DeconstructEdge(E edge);

        /// <summary>Gets all edges in this graph. If an edge (x, y) is added to this graph it may be returned as (y, x).</summary>
        public IEnumerable<E> Edges
        {
            get
            {
                var seenEdges = new HashSet<E>(new UndirectedEdgeEqualityComparer<E, V>(this.vertexComparer));
                foreach (var sourcePair in this.edges)
                {
                    foreach (VertexState targetState in sourcePair.Value)
                    {
                        var edge = ReconstructEdge(sourcePair.Key, targetState.Vertex, targetState.State);
                        if (!seenEdges.Contains(edge))
                        {
                            yield return edge;
                            bool added = seenEdges.Add(edge);
                            Debug.Assert(added, "Failed to add unseen edge to seen edges");
                        }
                    }
                }
            }
        }

        /// <summary>Gets all vertices in this graph.</summary>
        public IEnumerable<V> Vertices
        {
            get { return this.edges.Keys; }
        }

        public IEqualityComparer<V> VertexComparer
        {
            get { return this.vertexComparer; }
        }

        private void AddDirected(V source, V dest, E edge)
        {
            var adj = this.edges.GetOrInsert(source, NewAdjacencySet);
            VState edgeState = DeconstructEdge(edge);
            adj.Add(new VertexState(dest, edgeState));
        }

        private HashSet<VertexState> GetAdjacencySetFor(V vertex)
        {
            return this.edges.GetOr(vertex, NewAdjacencySet);
        }

        public abstract IEqualityComparer<E> EdgeComparer { get; }

        private HashSet<VertexState> NewAdjacencySet()
        {
            return new HashSet<VertexState>(new VertexStateComparer(this.vertexComparer));
        }

        //private struct WeightedVert
        //{
        //    public WeightedVert(V vertex, W weight)
        //        : this()
        //    {
        //        this.Vertex = vertex;
        //        this.Weight = weight;
        //    }

        //    public V Vertex { get; private set; }
        //    public W Weight { get; private set; }
        //}

        private class VertexStateComparer : IEqualityComparer<VertexState>
        {
            private readonly IEqualityComparer<V> vertexComparer;

            public VertexStateComparer(IEqualityComparer<V> vertexComparer)
            {
                this.vertexComparer = vertexComparer;
            }

            public bool Equals(VertexState x, VertexState y)
            {
                return this.vertexComparer.Equals(x.Vertex, y.Vertex);
            }

            public int GetHashCode(VertexState obj)
            {
                return this.vertexComparer.GetHashCode(obj.Vertex);
            }
        }

    }
}
