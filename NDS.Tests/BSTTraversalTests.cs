using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class BSTTraversalTests
    {
        [Test]
        public void Should_Traverse_In_Order()
        {
            var expected = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(1, "d"),
                new KeyValuePair<int, string>(2, "b"),
                new KeyValuePair<int, string>(3, "e"),
                new KeyValuePair<int, string>(4, "a"),
                new KeyValuePair<int, string>(6, "c"),
                new KeyValuePair<int, string>(9, "f"),
            };

            CollectionAssert.AreEqual(expected, BSTTraversal.InOrder(Create()));
        }

        [Test]
        public void Should_Traverse_Post_Order()
        {
            var expected = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(1, "d"),
                new KeyValuePair<int, string>(3, "e"),
                new KeyValuePair<int, string>(2, "b"),
                new KeyValuePair<int, string>(9, "f"),
                new KeyValuePair<int, string>(6, "c"),
                new KeyValuePair<int, string>(4, "a"),
            };

            CollectionAssert.AreEqual(expected, BSTTraversal.PostOrder(Create()));
        }

        [Test]
        public void Should_Traverse_Pre_Order()
        {
            var expected = new KeyValuePair<int, string>[]
            {
                new KeyValuePair<int, string>(4, "a"),
                new KeyValuePair<int, string>(2, "b"),
                new KeyValuePair<int, string>(1, "d"),
                new KeyValuePair<int, string>(3, "e"),
                new KeyValuePair<int, string>(6, "c"),
                new KeyValuePair<int, string>(9, "f"),
            };

            CollectionAssert.AreEqual(expected, BSTTraversal.PreOrder(Create()));
        }

        /// <summary>Test binary search tree for tests.</summary>
        /// <returns>
        ///        (4, a)
        ///       /      \
        ///    (2, b)   (6, c)
        ///   /    |        \
        ///(1, d) (3, e)   (9, f)
        /// </returns>
        private static BSTNode<int, string> Create()
        {
            var d = BSTNode.Create(1, "d");
            var e = BSTNode.Create(3, "e");
            var f = BSTNode.Create(9, "f");
            var b = BSTNode.Create(2, "b", left: d, right: e);
            var c = BSTNode.Create(6, "c", left: null, right: f);
            return BSTNode.Create(4, "a", left: b, right: c);
        }
    }
}
