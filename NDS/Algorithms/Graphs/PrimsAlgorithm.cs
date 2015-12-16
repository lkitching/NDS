using System;
using System.Collections.Generic;

using NDS.Graphs;
using NDS.Algorithms.Searching;

namespace NDS.Algorithms.Graphs
{
    public class PrimsAlgorithm
    {
        /// <summary>Finds a minimum spanning tree for a graph using Prim's algorithm.</summary>
        /// <typeparam name="E">Edge type of the graph.</typeparam>
        /// <typeparam name="V">Vertex type of the graph.</typeparam>
        /// <typeparam name="W">Weight type for the edges in the graph.</typeparam>
        /// <param name="graph">The graph to find a minimum spanning tree for.</param>
        /// <param name="weightComparer">Comparer for edge weights.</param>
        /// <returns>All the edges in the source graph which form the minimum spanning tree.</returns>
        public IEnumerable<E> MinimumSpanningTree<E, V, W>(IGraph<E, V> graph, IComparer<W> weightComparer)
            where E : IEdge<V>, IHasWeight<W>
        {
            var spanningTreeEdges = new HashSet<E>(graph.EdgeComparer);
            var minEdges = new BinaryHeap<VertexDistance<E, V, W>>(new VertexDistanceComparer<E, V, W>(weightComparer));
            var unprocessed = new HashSet<V>(graph.Vertices, graph.VertexComparer);
            var unprocessedVertexMinDistances = new Dictionary<V, W>(graph.VertexComparer);

            foreach (V vertex in graph.Vertices)
            {
                //all vertices initially unknown distance from the MST
                minEdges.Insert(new VertexDistance<E, V, W>(vertex, Maybe.None<E>()));
            }

            while (unprocessed.Count > 0)
            {
                var minDistance = minEdges.RemoveMinimum();
                V vertex = minDistance.Vertex;

                //NOTE: The queue can contain multiple edges for each vertex in the graph so the vertex corresponding to this
                //edge may already have been added to the MST
                if (!unprocessed.Contains(vertex)) continue;

                E minEdge;
                if (minDistance.MinEdge.HasValue)
                {
                    minEdge = minDistance.MinEdge.Value;
                }
                else
                {
                    //current vertex is an unknown distance to the MST. For a connected graph this should only
                    //occur on the first iteration when the MST is empty. Otherwise there are multiple connected
                    //components in the source graph
                    Maybe<E> minAdjacentEdge = graph.GetAdjacentEdges(vertex).MinimumBy(e => e.Weight, weightComparer);
                    if (minAdjacentEdge.HasValue)
                    {
                        minEdge = minAdjacentEdge.Value;
                    }
                    else
                    {
                        //no edges adjacent to this vertex so can't add an edge to the MST
                        //TODO: change the return type to include the set of vertices along with the set of edges?
                        //then this vertex could be added to the vertex set but not the edge set?
                        throw new ArgumentException("Found unconnected vertex in the graph");
                    }
                }

                //add the current min edge to the MST
                spanningTreeEdges.Add(minEdge);

                //no longer need to track min distance from vertex to MST
                unprocessedVertexMinDistances.Remove(vertex);
                
                //find all edges adjacent to the current vertex
                //if the weight of the edge vertex -> w is less than the current
                //edge between w and the current MST, update the min distance and
                //associated edge for w
                foreach (E edge in graph.GetAdjacentEdges(vertex))
                {
                    V w = GetOtherVertex<E, V>(vertex, edge, graph.VertexComparer);

                    if (unprocessed.Contains(w))
                    {
                        W currentMinWeight;
                        bool currentIsMin;
                        if (unprocessedVertexMinDistances.TryGetValue(w, out currentMinWeight))
                        {
                            currentIsMin = (weightComparer.CompareResult(edge.Weight, currentMinWeight) == ComparisonResult.Less);
                        }
                        else
                        {
                            currentIsMin = true;
                        }

                        if (currentIsMin)
                        {
                            //update min edge between w and the MST
                            //just add a new entry for the target vertex. Since it is smaller than the current minimum it
                            //is guaranteed to be processed first unless an even smaller edge is found
                            var newMinDistance = new VertexDistance<E, V, W>(w, Maybe.Some(edge));
                            minEdges.Insert(newMinDistance);
                            unprocessedVertexMinDistances[w] = edge.Weight;
                        }
                    }
                }

                //processed vertex
                bool removed = unprocessed.Remove(vertex);
            }

            return spanningTreeEdges;
        }

        private static V GetOtherVertex<E, V>(V vertex, E edge, IEqualityComparer<V> vertexComparer)
            where E : IEdge<V>
        {
            return vertexComparer.Equals(edge.V1, vertex)
                ? edge.V2
                : edge.V1;
        }

        private struct VertexDistance<E, V, W>
        {
            public VertexDistance(V vertex, Maybe<E> minEdge)
                : this()
            {
                this.Vertex = vertex;
                this.MinEdge = minEdge;
            }

            public V Vertex { get; private set; }

            public Maybe<E> MinEdge { get; private set; }
        }

        private class VertexDistanceComparer<E, V, W> : IComparer<VertexDistance<E, V, W>>
            where E : IHasWeight<W>
        {
            private readonly IComparer<W> weightComparer;

            public VertexDistanceComparer(IComparer<W> weightComparer)
            {
                this.weightComparer = weightComparer;
            }

            public int Compare(VertexDistance<E, V, W> x, VertexDistance<E, V, W> y)
            {
                var xEdge = x.MinEdge;
                var yEdge = y.MinEdge;

                if (xEdge.HasValue)
                {
                    if (yEdge.HasValue)
                    {
                        return this.weightComparer.Compare(xEdge.Value.Weight, yEdge.Value.Weight);
                    }
                    else return -1;
                }
                else
                {
                    //None is always larger than a known value
                    //None values are equal
                    return yEdge.HasValue ? 1 : 0;
                }
            }
        }
    }
}
