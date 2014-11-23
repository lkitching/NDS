using System;
using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="SinglyLinkedList{T}"/>.</summary>
    [TestFixture]
    public class SinglyLinkedListTests
    {
        /// <summary>Tests the list is initially empty.</summary>
        [Test]
        public void ShouldBeEmptyInitially()
        {
            var list = new SinglyLinkedList<int>();
            Assert.IsNull(list.First);
            Assert.IsTrue(list.IsEmpty);
        }

        /// <summary>Tests a node is inserted at the front of the list.</summary>
        [Test]
        public void ShouldInsertNode()
        {
            var list = new SinglyLinkedList<string>();
            var node = new SinglyLinkedListNode<string>("value");
            list.AddFirst(node);

            Assert.AreEqual(node, list.First, "Failed to insert node at front");
        }

        /// <summary>Tests a value is inserted at the front of the list.</summary>
        [Test]
        public void ShouldInsertValue()
        {
            var list = new SinglyLinkedList<int>();
            int value = new Random().Next();
            list.AddFirst(value);

            Assert.AreEqual(value, list.First.Value, "Failed to insert value at front");
        }

        /// <summary>Tests the first node is removed from the list.</summary>
        [Test]
        public void ShouldRemoveFirst()
        {
            var list = new SinglyLinkedList<int>();
            var node = new SinglyLinkedListNode<int>(new Random().Next());
            list.AddFirst(node);

            var removed = list.RemoveFirst();
            Assert.AreEqual(node, removed, "Removed unexpected node");
            Assert.IsTrue(list.IsEmpty);
        }

        /// <summary>Tests an exception is thrown when removing if the list is empty.</summary>
        [Test]
        public void RemoveFirstShouldThrowIfListEmpty()
        {
            var list = new SinglyLinkedList<string>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.RemoveFirst(); });
        }

        /// <summary>Tests the nodes in the list can be enumerated.</summary>
        [Test]
        public void ShouldEnumerateNodes()
        {
            var r = new Random();
            var count = r.Next(5, 100);
            var nodes = new SinglyLinkedListNode<int>[count];
            var list = new SinglyLinkedList<int>();

            for (int i = count - 1; i >= 0; --i)
            {
                var n = new SinglyLinkedListNode<int>(r.Next());
                nodes[i] = n;
                list.AddFirst(n);
            }

            CollectionAssert.AreEqual(nodes, list, "Unexpected nodes in list");
        }
    }
}
