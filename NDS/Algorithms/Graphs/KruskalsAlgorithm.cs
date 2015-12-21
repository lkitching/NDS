using System.Collections.Generic;
using System.Linq;
using NDS.Graphs;

namespace NDS.Algorithms.Graphs
{
    public class KruskalsAlgorithm : IMinimumSpanningTreeAlgorithm
    {
        public IEnumerable<E> MinimumSpanningTree<E, V, W>(IGraph<E, V> graph, IComparer<W> weightComparer)
            where E : IEdge<V>, IHasWeight<W>
        {
            var vertexComparer = graph.VertexComparer;
            var connectedVertices = new ForestDisjointSet<V>(vertexComparer);
            foreach (V vertex in graph.Vertices)
            {
                connectedVertices.MakeSet(vertex);
            }

            //iterate through the edges with increasing weight values
            //at each stage add an edge to the MST if it would not introduce a cycle
            //a cycle would be introduced if the two vertices incident on the edge are already connected by the MST
            //the disjoint set tracks the connectivity of vertices encountered so far - two vertices are connected
            //in the MST (or minimum-spanning forest if the graph is not connected) if they have the same representitive
            //in the dijoint set
            foreach (E edge in graph.Edges.OrderBy(e => e.Weight, weightComparer))
            {
                //NOTE: both should have representatives in the set since all vertices have been added to it
                //this should only fail if the graph contains edges with vertices not in its Vertices collection
                var v1Rep = connectedVertices.FindRepresentative(edge.V1).Value;
                var v2Rep = connectedVertices.FindRepresentative(edge.V2).Value;

                if (! vertexComparer.Equals(v1Rep, v2Rep))
                {
                    //V1 and V2 not already connected in the MST so add the edge to the MST and add the new connection
                    //between the vertices
                    yield return edge;
                    connectedVertices.Merge(edge.V1, edge.V2);
                }
            }
        }
    }
}
