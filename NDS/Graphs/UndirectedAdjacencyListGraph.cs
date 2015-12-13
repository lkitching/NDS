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
    public class UndirectedAdjacencyListGraph<V> : IGraph<V>
    {
        private readonly IEqualityComparer<V> comp;
        private readonly Dictionary<V, HashSet<V>> edges;

        /// <summary>Creates a new empty graph with the default equality comparer for the vertex type.</summary>
        public UndirectedAdjacencyListGraph()
            : this(Enumerable.Empty<UndirectedEdge<V>>(), EqualityComparer<V>.Default)
        {
        }

        /// <summary>Creates an empty graph with the given equality comparer for the vertex type.</summary>
        /// <param name="vertexComparer">Comparer for vertices in the graph.</param>
        public UndirectedAdjacencyListGraph(IEqualityComparer<V> vertexComparer)
            : this(Enumerable.Empty<UndirectedEdge<V>>(), vertexComparer)
        {
        }

        /// <summary>Creates a graph containing the given edges and the default comparer for the vertex type.</summary>
        /// <param name="edges">The edges to add to the graph.</param>
        public UndirectedAdjacencyListGraph(IEnumerable<UndirectedEdge<V>> edges)
            : this(edges, EqualityComparer<V>.Default)
        {
        }

        /// <summary>Creates a graph containing the given edges and comparer for the vertex type.</summary>
        /// <param name="edges">The edges to add to the graph.</param>
        /// <param name="vertexComparer">The equality comparer to use for vertices.</param>
        public UndirectedAdjacencyListGraph(IEnumerable<UndirectedEdge<V>> edges, IEqualityComparer<V> vertexComparer)
        {
            this.comp = vertexComparer;
            this.edges = new Dictionary<V, HashSet<V>>(comp);

            foreach (var edge in edges)
            {
                this.AddEdge(edge);
            }
        }

        /// <summary>Adds the given edge to this graph.</summary>
        /// <param name="edge">The edge to add.</param>
        public void AddEdge(UndirectedEdge<V> edge)
        {
            AddDirected(edge.X, edge.Y);
            AddDirected(edge.Y, edge.X);
        }

        /// <summary>Removes the given edge from this graph if it exists.</summary>
        /// <param name="edge"></param>
        public void RemoveEdge(UndirectedEdge<V> edge)
        {
            this.RemoveAdjacency(edge.X, edge.Y);
            this.RemoveAdjacency(edge.Y, edge.X);
        }

        /// <summary>Removes the given vertex and all incident edges from this graph.</summary>
        /// <param name="vertex">The vertex to remove.</param>
        /// <returns>True if the vertex existed in this graph, otherwise false.</returns>
        public bool RemoveVertex(V vertex)
        {
            HashSet<V> adj;
            if (this.edges.TryGetValue(vertex, out adj))
            {
                this.edges.Remove(vertex);
                foreach (V target in adj)
                {
                    this.RemoveAdjacency(target, vertex);
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
            adj.Remove(target);
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
            return this.edges.GetOrValue(vertex, null).EmptyIfNull();
        }

        /// <summary>Finds all edges incident on a given vertex.</summary>
        /// <param name="vertex">The vertex to find incident edges for.</param>
        /// <returns>All edges incident on the given vertex.</returns>
        public IEnumerable<UndirectedEdge<V>> GetAdjacentEdges(V vertex)
        {
            var adjacentVertices = GetAdjacencySetFor(vertex);
            foreach (V adj in adjacentVertices)
            {
                yield return new UndirectedEdge<V>(vertex, adj);
            }
        }

        /// <summary>Gets all edges in this graph. If an edge (x, y) is added to this graph it may be returned as (y, x).</summary>
        public IEnumerable<UndirectedEdge<V>> Edges
        {
            get
            {
                var seenEdges = new HashSet<UndirectedEdge<V>>(new UndirectedEdgeEqualityComparer<V>(this.comp));
                foreach (var sourcePair in this.edges)
                {
                    foreach (V targetVertex in sourcePair.Value)
                    {
                        var edge = new UndirectedEdge<V>(sourcePair.Key, targetVertex);
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
            get { return this.comp; }
        }

        private void AddDirected(V source, V dest)
        {
            var adj = this.edges.GetOrInsert(source, () => new HashSet<V>());
            adj.Add(dest);
        }

        private HashSet<V> GetAdjacencySetFor(V vertex)
        {
            return this.edges.GetOr(vertex, () => new HashSet<V>());
        }
    }
}
