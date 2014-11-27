using System;
using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="DoublyLinkedListNode{T}"/>.</summary>
    [TestFixture]
    public class DoublyLinkedListNodeTests
    {
        [Test]
        public void ConstructorShouldSetValue()
        {
            int value = new Random().Next();
            var node = new DoublyLinkedListNode<int>(value);
            Assert.AreEqual(value, node.Value, "Failed to set value");
        }

        /// <summary>Tests a node is inserted before another node.</summary>
        [Test]
        public void ShouldInsertBefore()
        {
            var node = Create("value");
            var next = Create("next");

            node.InsertBefore(next);

            Assert.AreEqual(node.Next, next, "Failed to set next");
            Assert.AreEqual(next.Previous, node, "Failed to set predecessor of next node");
        }

        /// <summary>Tests a node is inserted after another node.</summary>
        [Test]
        public void ShouldInsertAfter()
        {
            var node = Create("node");
            var previous = Create("previous");

            node.InsertAfter(previous);

            Assert.AreEqual(previous, node.Previous, "Failed to set previous");
            Assert.AreEqual(node, previous.Next, "Failed to set next of predecessor");
        }

        /// <summary>Tests a node removes itself from a list and connects its neighbours.</summary>
        [Test]
        public void ShouldUnlink()
        {
            var previous = Create("pred");
            var next = Create("succ");
            var node = new DoublyLinkedListNode<string>("value");

            node.InsertBefore(next);
            node.InsertAfter(previous);

            node.Unlink();
            Assert.AreEqual(next, previous.Next, "Failed connect neighbours");
            Assert.AreEqual(previous, next.Previous, "Failed to connect neighbours");
        }

        private static DoublyLinkedListNode<T> Create<T>(T value)
        {
            return new DoublyLinkedListNode<T>(value);
        }
    }
}
