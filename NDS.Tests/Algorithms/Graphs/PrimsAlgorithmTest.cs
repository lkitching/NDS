using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NDS.Graphs;
using NDS.Algorithms.Graphs;
using NUnit.Framework;

namespace NDS.Tests.Algorithms.Graphs
{
    [TestFixture]
    public class PrimsAlgorithmTest
    {
        [Test]
        public void Should_Find_MST()
        {
            var ab = new WeightedUndirectedEdge<char, int>('A', 'B', 3);
            var ac = new WeightedUndirectedEdge<char, int>('A', 'C', 2);
            var bc = new WeightedUndirectedEdge<char, int>('B', 'C', 4);
            var cd = new WeightedUndirectedEdge<char, int>('C', 'D', 5);
            var ad = new WeightedUndirectedEdge<char, int>('A', 'D', 8);
            var be = new WeightedUndirectedEdge<char, int>('B', 'E', 1);
            var de = new WeightedUndirectedEdge<char, int>('D', 'E', 1);

            var edges = new[] { ab, ac, bc, cd, ad, be, de };

            var graph = new WeightedUndirectedAdjacencyListGraph<char, int>(edges, EqualityComparer<char>.Default, EqualityComparer<int>.Default);
            var prims = new PrimsAlgorithm();
            var mstEdges = prims.MinimumSpanningTree(graph, Comparer<int>.Default).ToArray();

            var expectedEdges = new[] { ab, ac, de, be };
            var comp = new HasWeightEqualityComparer<WeightedUndirectedEdge<char, int>, int>(new UndirectedEdgeEqualityComparer<WeightedUndirectedEdge<char, int>, char>(), EqualityComparer<int>.Default);
            TestAssert.SetEqual(expectedEdges, mstEdges, comp);
        }
    }
}
