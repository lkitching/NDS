using System;
using System.Collections.Generic;
using NDS.Graphs;

namespace NDS.Algorithms.Graphs
{
    /// <summary>Searches a graph depth-first.</summary>
    public class DepthFirstSearch
    {
        /// <summary>Searches the given graph depth-first from a start vertex.</summary>
        /// <typeparam name="V">The vertex type of the graph.</typeparam>
        /// <param name="graph">The graph to search.</param>
        /// <param name="startVertex">The vertex to start from.</param>
        /// <returns>The sequence of vertices in the order they were visited by the search.</returns>
        public IEnumerable<V> Search<E, V>(IGraph<E, V> graph, V startVertex)
        {
            if (!graph.ContainsVertex(startVertex)) yield break;

            var path = new DynamicStack<V>();
            path.Push(startVertex);
            var visited = new HashSet<V>(graph.VertexComparer) { startVertex };

            while (path.Count > 0)
            {
                V top = path.Peek();
                bool found = false;
                foreach (V neighbour in graph.GetAdjacentVertices(top))
                {
                    if (!visited.Contains(neighbour))
                    {
                        path.Push(neighbour);
                        visited.Add(neighbour);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    //if no neighbours of the current node are unvisited then yield the current node
                    //and continue
                    path.Pop();
                    yield return top;
                }
            }
        }
    }
}
