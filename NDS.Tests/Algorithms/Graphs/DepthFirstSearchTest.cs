using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NDS.Graphs;
using NDS.Algorithms.Graphs;

namespace NDS.Tests.Algorithms.Graphs
{
    public class DepthFirstSearchTest
    {
        [Test]
        public void Empty_Graph_Test()
        {
            var graph = new UndirectedAdjacencyListGraph<char>();
            var search = new DepthFirstSearch();
            var vertices = search.Search(graph, 'A');

            CollectionAssert.IsEmpty(vertices);
        }

        [Test]
        public void Should_Visit_Vertices_Depth_First()
        {
            var graph = new UndirectedAdjacencyListGraph<char>(new[] {
                new UndirectedEdge<char>('A', 'B'),
                new UndirectedEdge<char>('A', 'E'),
                new UndirectedEdge<char>('B', 'C'),
                new UndirectedEdge<char>('B', 'D'),
                new UndirectedEdge<char>('D', 'E')
            });

            var search = new DepthFirstSearch();
            var vertices = search.Search(graph, 'A').ToArray();

            var expectedPath = new[] { 'C', 'E', 'D', 'B', 'A' };
            CollectionAssert.AreEqual(expectedPath, vertices);
        }
    }
}
