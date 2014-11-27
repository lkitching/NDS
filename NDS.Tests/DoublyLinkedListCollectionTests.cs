using System;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="DoublyLinkedListCollection{T}"/>.</summary>
    [TestFixture]
    public class DoublyLinkedListCollectionTests
    {
        /// <summary>Tests a new collection has a count of 0.</summary>
        [Test]
        public void ShouldBeInitiallyEmpty()
        {
            var list = new DoublyLinkedListCollection<int>();
            Assert.AreEqual(0, list.Count, "Initial count should be 0");
        }

        /// <summary>Tests the count is incremented after an item is inserted.</summary>
        [Test]
        public void ShouldIncrementCountOnInsert()
        {
            var list = new DoublyLinkedListCollection<int>();
            int count = 10;

            foreach (int i in Enumerable.Range(1, count))
            {
                list.AddFirst(i);
            }

            Assert.AreEqual(count, list.Count, "Failed to increment count on insert");
        }

        /// <summary>Tests the first item can be retrieved.</summary>
        [Test]
        public void ShouldGetFirst()
        {
            var first = "first";
            var list = new DoublyLinkedListCollection<string>();
            list.AddFirst(first);

            Assert.AreEqual(first, list.First, "Unexpected first value");
        }

        /// <summary>Tests an exception is thrown when trying to fetch the first item when the list is empty.</summary>
        [Test]
        public void FirstGetterShouldThrowIfListEmpty()
        {
            var list = new DoublyLinkedListCollection<int>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.First; });
        }

        /// <summary>Tests the last item can be retrieved.</summary>
        [Test]
        public void ShouldGetLast()
        {
            var last = "last";
            var list = new DoublyLinkedListCollection<string>();
            list.AddLast(last);

            Assert.AreEqual(last, list.Last, "Unexpected last value");
        }

        /// <summary>Tests an exception is thrown when trying to fetch the last item when the list is empty.</summary>
        [Test]
        public void LastGetterShouldThrowIfListEmpty()
        {
            var list = new DoublyLinkedListCollection<long>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.Last; });
        }

        /// <summary>Tests the first item can be removed.</summary>
        [Test]
        public void ShouldRemoveFirst()
        {
            string first = "first";
            var list = Create(first, "second", "third");

            var removed = list.RemoveFirst();
            Assert.AreEqual(first, removed, "Unexpected first element removed");
        }

        /// <summary>Tests an exception is thrown when trying to the remove the first element from an empty list.</summary>
        [Test]
        public void RemoveFirstShouldThrowIfEmpty()
        {
            var list = new DoublyLinkedListCollection<string>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.RemoveFirst(); });
        }

        /// <summary>Tests the last item can be removed.</summary>
        [Test]
        public void ShouldRemoveLast()
        {
            var last = "last";
            var list = Create("first", "second", last);

            var removed = list.RemoveLast();
            Assert.AreEqual(last, removed, "Unexpected last element removed");
        }

        /// <summary>Tests an exception is thrown when trying to the remove the last element from an empty list.</summary>
        [Test]
        public void RemoveLastShouldThrowIfEmpty()
        {
            var list = new DoublyLinkedListCollection<int>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.RemoveLast(); });
        }

        /// <summary>Tests the collection can be enumerated.</summary>
        [Test]
        public void ShouldEnumerateItems()
        {
            var r = new Random();
            var count = r.Next(200, 1000);
            var items = TestGen.RandomInts(r).Take(count).ToArray();

            var list = Create(items);
            CollectionAssert.AreEqual(items, list, "Unexpected items in list");
        }

        /// <summary>Tests the first item matching a predicate is removed.</summary>
        [Test]
        public void ShouldRemoveFirstItemMatching()
        {
            var list = Create(1, 2, 3, 2, 0);
            bool removed = list.RemoveFirstWhere(i => i % 2 == 0);

            Assert.IsTrue(removed, "Failed to remove item");
            Assert.AreEqual(4, list.Count, "Failed to adjust count");
        }

        /// <summary>Tests no items are removed if no items match the predicate.</summary>
        [Test]
        public void RemoveFirstWhereShouldReturnFalseIfNoneMatch()
        {
            int count = 10;
            var list = Create(Enumerable.Range(0, count).ToArray());

            bool removed = list.RemoveFirstWhere(i => i > count);
            Assert.IsFalse(removed, "Removed non-matching item");
            Assert.AreEqual(count, list.Count, "Unexpected count after remove failed");
        }

        /// <summary>Tests all items are removed from the collection if they match the predicate.</summary>
        [Test]
        public void ShouldRemoveAll()
        {
            var r = new Random();
            int count = r.Next(100, 200);
            var list = Create(Enumerable.Range(0, count).ToArray());

            int removed = list.RemoveAllWhere(i => i < count);
            Assert.AreEqual(count, removed, "Unexpected number of removed items");
            Assert.AreEqual(0, list.Count, "Failed to adjust count");
            CollectionAssert.AreEqual(new int[0], list, "Unexpected items in list");
        }

        /// <summary>Tests all items matching the predicate are removed.</summary>
        [Test]
        public void ShouldRemoveAllWhereMatching()
        {
            var r = new Random();
            int count = r.Next(500, 1000);
            var items = TestGen.RandomInts(r).Take(count).ToArray();
            var list = Create(items);
            Func<int, bool> p = i => i % 2 == 0;

            int removed = list.RemoveAllWhere(p);
            CollectionAssert.AreEqual(items.Where(i => !p(i)), list, "Unexpected items remaining after remove");
            Assert.AreEqual(items.Count(p), removed, "Unexpected number of items removed");
            Assert.AreEqual(count, removed + list.Count, "Failed to adjust count");
        }

        private static DoublyLinkedListCollection<T> Create<T>(params T[] items)
        {
            var list = new DoublyLinkedListCollection<T>();
            foreach (T item in items)
            {
                list.AddLast(item);
            }
            return list;
        }
    }
}
