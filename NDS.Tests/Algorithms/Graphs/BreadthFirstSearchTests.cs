using NDS.Algorithms.Graphs;
using NDS.Graphs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests.Algorithms.Graphs
{
    [TestFixture]
    public class BreadthFirstSearchTests
    {
        [Test]
        public void Empty_Graph_Test()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            var search = new BreadthFirstSearch();
            var vertices = search.Search(graph, 1);

            CollectionAssert.IsEmpty(vertices);
        }

        [Test]
        public void Should_Visit_Edges_Breadth_First()
        {
            var graph = new UndirectedAdjacencyListGraph<char>(new[] {
                new UndirectedEdge<char>('A', 'B'),
                new UndirectedEdge<char>('A', 'E'),
                new UndirectedEdge<char>('B', 'C'),
                new UndirectedEdge<char>('B', 'D'),
                new UndirectedEdge<char>('D', 'E')
            });

            var search = new BreadthFirstSearch();
            var vertices = search.Search(graph, 'A').ToArray();

            var expectedPath = new[] { 'A', 'B', 'E', 'C', 'D' };
            CollectionAssert.AreEqual(expectedPath, vertices);
        }
    }
}
