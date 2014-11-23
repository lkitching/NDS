using System;
using NUnit.Framework;
using System.Linq;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="SinglyLinkedListNode{T}"/>.</summary>
    [TestFixture]
    public class SinglyLinkedListNodeTests
    {
        /// <summary>Tests the constructor sets the node value.</summary>
        [Test]
        public void ConstructorShouldSetValue()
        {
            int value = new Random().Next();
            var n = new SinglyLinkedListNode<int>(value);
            Assert.AreEqual(value, n.Value, "Failed to set value");
        }

        /// <summary>Tests the constructor sets the next node.</summary>
        [Test]
        public void ConstructorShouldSetNext()
        {
            var next = new SinglyLinkedListNode<string>("next");
            var n = new SinglyLinkedListNode<string>("first", next);
            Assert.AreEqual(n.Next, next, "Failed to set next");
        }

        /// <summary>Tests a node inserts itself after the given node.</summary>
        [Test]
        public void ShouldInsertAfter()
        {
            var pred = new SinglyLinkedListNode<string>("pred");
            var n = new SinglyLinkedListNode<string>("n");

            n.InsertAfter(pred);

            Assert.AreEqual(n, pred.Next, "Failed to insert after node");
        }

        /// <summary>Tests a node inserts itself between two given nodes.</summary>
        [Test]
        public void ShouldInsertBetween()
        {
            var pred = new SinglyLinkedListNode<string>("pred");
            var next = new SinglyLinkedListNode<string>("next");
            var n = new SinglyLinkedListNode<string>("n");

            n.InsertBetween(pred, next);

            Assert.AreEqual(n, pred.Next, "Failed to set predecessor node");
            Assert.AreEqual(next, n.Next, "Failed to set successor node");
        }

        /// <summary>Tests a node unlinks its successor.</summary>
        [Test]
        public void ShouldUnlinkNext()
        {
            var nnext = new SinglyLinkedListNode<string>("nnext");
            var next = new SinglyLinkedListNode<string>("next", nnext);
            var n = new SinglyLinkedListNode<string>("n", next);

            n.UnlinkNext();
            Assert.AreEqual(nnext, n.Next, "Failed to unlink node");
        }

        /// <summary>Tests a node can enumerate itself and all its successors.</summary>
        [Test]
        public void ShouldEnumerateFrom()
        {
            var third = new SinglyLinkedListNode<int>(3);
            var second = new SinglyLinkedListNode<int>(2, third);
            var first = new SinglyLinkedListNode<int>(1, second);

            CollectionAssert.AreEqual(new[] { first, second, third }, first.EnumerateFrom());
        }
    }
}
