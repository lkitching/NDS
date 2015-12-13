using System;
using System.Collections.Generic;
using NDS.Graphs;

namespace NDS.Algorithms.Graphs
{
    /// <summary>Searches a graph breadth-first.</summary>
    public class BreadthFirstSearch
    {
        /// <summary>Searches the given graph breadth-first from the start vertex./</summary>
        /// <typeparam name="V">The type of vertices in the graph.</typeparam>
        /// <param name="graph">The graph to search.</param>
        /// <param name="startVertex">The vertex to start the search from.</param>
        /// <returns>The vertices visited in breadth-first order.</returns>
        public IEnumerable<V> Search<V>(IGraph<V> graph, V startVertex)
        {
            if (!graph.ContainsVertex(startVertex)) yield break;

            var visitedNodes = new HashSet<V>(graph.VertexComparer);
            var remaining = new DynamicLinkedQueue<V>();
            remaining.Enqueue(startVertex);

            while (remaining.Count > 0)
            {
                V current = remaining.Dequeue();
                if (!visitedNodes.Contains(current))
                {
                    yield return current;
                    visitedNodes.Add(current);
                    foreach (V neighbour in graph.GetAdjacentVertices(current))
                    {
                        remaining.Enqueue(neighbour);
                    }
                }
            }
        }
    }
}
