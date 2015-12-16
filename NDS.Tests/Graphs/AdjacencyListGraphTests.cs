using System;
using System.Collections.Generic;
using System.Linq;

using NDS.Graphs;
using NUnit.Framework;

namespace NDS.Tests.Graphs
{
    [TestFixture]
    public class AdjacencyListGraphTests
    {
        [Test]
        public void Vertices_Should_Be_Empty_On_Create()
        {
            var graph = new UndirectedAdjacencyListGraph<int>();
            CollectionAssert.IsEmpty(graph.Vertices, "Should be empty on creation");
        }

        [Test]
        public void Should_Add_Edge_Vertices()
        {
            var graph = Create<int>();
            int v1 = 4;
            int v2 = 2;

            graph.AddEdge(new UndirectedEdge<int>(v1, v2));

            CollectionAssert.AreEquivalent(new[] { v1, v2 }, graph.Vertices, "Unexpected vertices after adding edge");
        }

        [Test]
        public void Should_Add_Edges()
        {
            var graph = Create<int>();
            var vertices = TestGen.NRandomInts(2000, 2000).ToArray();
            var edgeComparer = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();
            var edges = new HashSet<UndirectedEdge<int>>(edgeComparer);

            for (int i = 0; i < vertices.Length; i += 2)
            {
                var edge = new UndirectedEdge<int>(vertices[i], vertices[i + 1]);
                edges.Add(edge);
                graph.AddEdge(edge);
            }

            TestAssert.SetEqual(edges, graph.Edges, edgeComparer);
        }

        [Test]
        public void Should_Remove_Edge()
        {
            var e1 = new UndirectedEdge<int>(1, 2);
            var e2 = new UndirectedEdge<int>(1, 3);
            var e3 = new UndirectedEdge<int>(2, 3);

            var graph = new UndirectedAdjacencyListGraph<int>(new[] { e1, e2, e3 });
            graph.RemoveEdge(e2);

            TestAssert.SetEqual(new[] { e1, e3 }, graph.Edges, message: "Unexpected edges after removal");
        }

        [Test]
        public void Should_Remove_Vertex_If_Last_Edge_Removed()
        {
            var e1 = new UndirectedEdge<int>(1, 2);
            var e2 = new UndirectedEdge<int>(1, 3);
            var e3 = new UndirectedEdge<int>(3, 4);

            var graph = new UndirectedAdjacencyListGraph<int>(new[] { e1, e2, e3 });
            graph.RemoveEdge(e1);

            CollectionAssert.AreEquivalent(new[] { 1, 3, 4 }, graph.Vertices, "Unexpected vertices after removing edge");
        }

        [Test]
        public void Should_Remove_Vertex()
        {
            var random = new Random();
            int v1 = random.Next();
            int v2 = random.Next();
            int v3 = random.Next();

            var graph = new UndirectedAdjacencyListGraph<int>(new[] {
                new UndirectedEdge<int>(v1, v2),
                new UndirectedEdge<int>(v1, v3)
            });

            graph.RemoveVertex(v3);
            CollectionAssert.AreEquivalent(new[] { v1, v2 }, graph.Vertices, "Unexpected vertices after removal");
        }

        [Test]
        public void Should_Remove_All_Edges_Indicident_On_Vertex()
        {
            var random = new Random();
            int v1 = random.Next();
            int v2 = random.Next();
            int v3 = random.Next();

            var e1 = new UndirectedEdge<int>(v1, v2);
            var e2 = new UndirectedEdge<int>(v1, v3);
            var e3 = new UndirectedEdge<int>(v2, v3);

            var graph = new UndirectedAdjacencyListGraph<int>(new[] { e1, e2, e3 });

            graph.RemoveVertex(v1);
            CollectionAssert.AreEquivalent(new[] { e3 }, graph.Edges, "Unexpected edges after removing vertex");
        }

        [Test]
        public void Should_Get_Adjacent_Vertices()
        {
            var random = new Random();
            int v1 = random.Next();
            int v2 = random.Next();
            int v3 = random.Next();
            int v4 = random.Next();

            var e1 = new UndirectedEdge<int>(v1, v2);
            var e2 = new UndirectedEdge<int>(v1, v3);
            var e3 = new UndirectedEdge<int>(v2, v3);
            var e4 = new UndirectedEdge<int>(v4, v1);

            var graph = new UndirectedAdjacencyListGraph<int>(new[] { e1, e2, e3, e4 });

            CollectionAssert.AreEquivalent(new[] { v2, v3, v4 }, graph.GetAdjacentVertices(v1));
            CollectionAssert.AreEquivalent(new[] { v1, v3 }, graph.GetAdjacentVertices(v2));
            CollectionAssert.AreEquivalent(new[] { v1, v2 }, graph.GetAdjacentVertices(v3));
            CollectionAssert.AreEquivalent(new[] { v1 }, graph.GetAdjacentVertices(v4));
        }

        [Test]
        public void Should_Get_Indicent_Edges()
        {
            var random = new Random();
            int v1 = random.Next();
            int v2 = random.Next();
            int v3 = random.Next();
            int v4 = random.Next();

            var e1 = new UndirectedEdge<int>(v1, v2);
            var e2 = new UndirectedEdge<int>(v1, v3);
            var e3 = new UndirectedEdge<int>(v2, v3);
            var e4 = new UndirectedEdge<int>(v4, v1);

            var graph = new UndirectedAdjacencyListGraph<int>(new[] { e1, e2, e3, e4 });
            var edgeComparer = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();

            TestAssert.SetEqual(new[] { e1, e2, e4 }, graph.GetAdjacentEdges(v1), edgeComparer);
            TestAssert.SetEqual(new[] { e1, e3 }, graph.GetAdjacentEdges(v2), edgeComparer);
            TestAssert.SetEqual(new[] { e2, e3 }, graph.GetAdjacentEdges(v3), edgeComparer);
            TestAssert.SetEqual(new[] { e4 }, graph.GetAdjacentEdges(v4), edgeComparer);
        }

        private static UndirectedAdjacencyListGraph<T> Create<T>()
        {
            return new UndirectedAdjacencyListGraph<T>();
        }
    }
}
