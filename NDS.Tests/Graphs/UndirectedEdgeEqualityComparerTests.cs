using System;
using NDS.Graphs;
using NUnit.Framework;

namespace NDS.Tests.Graphs
{
    [TestFixture]
    public class UndirectedEdgeEqualityComparerTests
    {
        [Test]
        public void Edges_With_Equal_Corresponding_Vertices_Should_Be_Equal()
        {
            var e1 = RandomEdge();
            var e2 = new UndirectedEdge<int>(e1.V1, e1.V2);
            var comp = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();

            Assert.IsTrue(comp.Equals(e1, e2), "Edges should be equal under comparer");
        }

        [Test]
        public void Edges_With_Equal_Opposite_Vertices_Should_Be_Equal()
        {
            var e1 = RandomEdge();
            var e2 = new UndirectedEdge<int>(e1.V1, e1.V2);
            var comp = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();

            Assert.IsTrue(comp.Equals(e1, e2), "Edges should be equal under comparer");
        }

        [Test]
        public void Edges_Should_Not_Be_Equal()
        {
            int v = new Random().Next();
            var e1 = new UndirectedEdge<int>(v, v + 100);
            var e2 = new UndirectedEdge<int>(v - 100, v + 500);

            var comp = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();
            Assert.IsFalse(comp.Equals(e1, e2), "Edges should not be equal");
        }

        [Test]
        public void Equal_Edges_With_Corresponding_Vertices_Should_Have_Same_Hash_Code()
        {

            var e1 = RandomEdge();
            var e2 = new UndirectedEdge<int>(e1.V1, e1.V2);
            var comp = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();

            Assert.AreEqual(comp.GetHashCode(e1), comp.GetHashCode(e2), "Edges should have the same hash code");
        }

        [Test]
        public void Equal_Edges_With_Opposite_Vertices_Should_Have_Same_Hash_Code()
        {
            var e1 = RandomEdge();
            var e2 = new UndirectedEdge<int>(e1.V2, e1.V1);
            var comp = new UndirectedEdgeEqualityComparer<UndirectedEdge<int>, int>();

            Assert.AreEqual(comp.GetHashCode(e1), comp.GetHashCode(e2), "Edges should have the same hash code");
        }

        private static UndirectedEdge<int> RandomEdge()
        {
            var random = new Random();
            int v1 = random.Next();
            int v2 = random.Next();
            return new UndirectedEdge<int>(v1, v2);
        }
    }
}
