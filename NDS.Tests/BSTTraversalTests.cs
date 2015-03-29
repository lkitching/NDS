using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class BSTTraversalTests
    {
        private BSTNode<int, string> A, B, C, D, E, F;

        /// <summary>Creates the binary search tree for tests.</summary>
        /// <returns>
        ///        (4, a)
        ///       /      \
        ///    (2, b)   (6, c)
        ///   /    |        \
        ///(1, d) (3, e)   (9, f)
        /// </returns>
        [SetUp]
        public void Setup()
        {
            D = BSTNode.Create(1, "d");
            E = BSTNode.Create(3, "e");
            F = BSTNode.Create(9, "f");
            B = BSTNode.Create(2, "b", left: D, right: E);
            C = BSTNode.Create(6, "c", left: null, right: F);
            A = BSTNode.Create(4, "a", left: B, right: C);
        }

        [Test]
        public void Should_Traverse_In_Order()
        {
            var expected = new BSTNode<int, string>[] { D, B, E, A, C, F };
            CollectionAssert.AreEqual(expected, BSTTraversal.InOrder(A));
        }

        [Test]
        public void Should_Traverse_Post_Order()
        {
            var expected = new BSTNode<int, string>[] { D, E, B, F, C, A };
            CollectionAssert.AreEqual(expected, BSTTraversal.PostOrder(A));
        }

        [Test]
        public void Should_Traverse_Pre_Order()
        {
            var expected = new BSTNode<int, string>[] { A, B, D, E, C, F };
            CollectionAssert.AreEqual(expected, BSTTraversal.PreOrder(A));
        }
    }
}
