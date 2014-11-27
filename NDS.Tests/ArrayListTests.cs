using System;
using System.Collections.Generic;
using NUnit.Framework;

using System.Linq;

namespace NDS.Tests
{
    [TestFixture]
    public class ArrayListTests
    {
        [Test]
        public void ConstructorShouldThrowIfInitialCapacityNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { var sut = new ArrayList<int>(-1); });
        }

        [Test]
        public void ShouldBeEmptyInitially()
        {
            var sut = Create<int>();
            Assert.AreEqual(0, sut.Count, "Initial count should be empty");
        }

        [Test]
        public void ShouldAddItem()
        {
            int item = 1;
            var sut = Create<int>();

            sut.Add(item);
            Assert.AreEqual(item, sut[0], "Should add item at end of list");
        }

        [Test]
        public void ShouldAddItemBeyondCapacity()
        {
            int item = 4;
            var sut = new ArrayList<int>(3) { 1, 2, 3 };
            sut.Add(item);

            Assert.AreEqual(item, sut[3], "Should expand capacity and add to end of list");
        }

        [Test]
        public void CountShouldBeIncrementedAfterAdd()
        {
            var sut = Create<int>();
            sut.Add(1);

            Assert.AreEqual(1, sut.Count, "Should increment count after Add");
        }

        [Test]
        public void ShouldAddItemAtIndex()
        {
            var item = 4;
            var index = 1;

            var sut = new ArrayList<int> { 3, 5 };
            sut.Add(item, index);

            Assert.AreEqual(item, sut[index], "Should add item at given index");
        }

        [Test]
        public void ShouldContainItem()
        {
            var sut = new ArrayList<int> { 1, 3, 5 };
            Assert.IsTrue(sut.Contains(3), "Did not find contained item");
        }

        [Test]
        public void ShouldContainItemUsingComparer()
        {
            var sut = new ArrayList<string> { "a", "b", "c" };
            Assert.IsTrue(sut.Contains("B", StringComparer.OrdinalIgnoreCase));
        }

        [Test]
        public void ShouldNotContainItem()
        {
            var sut = new ArrayList<int> { 1, 3, 5 };
            Assert.IsFalse(sut.Contains(2), "Found item not in list");
        }

        [Test]
        public void ShouldRemoveItem()
        {
            var sut = new ArrayList<int> { 1, 2, 3 };
            bool removed = sut.RemoveFirst(2);

            Assert.IsTrue(removed, "Failed to remove contained item");
        }

        [Test]
        public void RemoveShouldDecrementCount()
        {
            var sut = new ArrayList<int> { 1, 2, 3 };
            sut.RemoveFirst(1);

            CollectionAssert.AreEqual(new[] { 2, 3 }, sut);
            Assert.AreEqual(2, sut.Count, "Failed to decrement count after Remove");
        }

        [Test]
        public void ShouldRemoveItemUsingComparer()
        {
            var sut = new ArrayList<string> { "a", "b", "c", "B" };
            sut.RemoveFirst("B", StringComparer.OrdinalIgnoreCase);

            //should only remove first 'b' (case-insensitively)
            CollectionAssert.AreEqual(new[] { "a", "c", "B" }, sut);
        }

        [Test]
        public void ShouldRemoveItemUsingPredicate()
        {
            var sut = new ArrayList<int> { 1, 2, 3, 4 };
            sut.RemoveFirstWhere(i => i % 2 == 0);

            //should only remove first even number
            CollectionAssert.AreEqual(new[] { 1, 3, 4 }, sut);
        }

        [Test]
        public void ShouldRemoveAll()
        {
            var sut = new ArrayList<int> { 1, 2, 3, 2 };
            int removed = sut.RemoveAll(2);

            Assert.AreEqual(2, removed, "Unexpected number of removed items");
            CollectionAssert.AreEqual(new[] { 1, 3 }, sut);
        }

        [Test]
        public void ShouldRemoveAllUsingComparer()
        {
            var sut = new ArrayList<string> { "a", "b", "B", "c", "b", "d" };
            int removed = sut.RemoveAll("b", StringComparer.OrdinalIgnoreCase);

            Assert.AreEqual(3, removed, "Unexpected number of removed items");
            CollectionAssert.AreEqual(new[] { "a", "c", "d" }, sut);
        }

        [Test]
        public void ShouldRemoveAllUsingPredicate()
        {
            var sut = new ArrayList<int>(Enumerable.Range(1, 10));
            int removed = sut.RemoveAllWhere(i => i > 5);

            Assert.AreEqual(5, removed, "Unexpected number of removed items");
            CollectionAssert.AreEqual(Enumerable.Range(1, 5), sut);
        }

        [Test]
        public void ShouldRemoveItemAtIndex()
        {
            var sut = new ArrayList<int> { 1, 2, 3 };

            int removed = sut.RemoveAt(1);

            Assert.AreEqual(2, removed, "Unexpected number of removed items");
            CollectionAssert.AreEqual(new[] { 1, 3 }, sut);
        }

        [Test]
        public void ShouldRemoveItemAtNegativeIndex()
        {
            var sut = new ArrayList<int> { 1, 2, 3, 4 };

            int removed = sut.RemoveAt(-2);
        }

        [Test]
        public void RemoveAtShouldThrowIfIndexTooLarge()
        {
            var sut = new ArrayList<int> { 1, 2, 3 };
            Assert.Throws<ArgumentOutOfRangeException>(() => { int removed = sut.RemoveAt(3); });
        }

        [Test]
        public void RemoveAtShouldThrowIfIndexTooSmall()
        {
            var sut = new ArrayList<int> { 1, 2, 3 };

            //largest valid negative index is -3
            Assert.Throws<ArgumentOutOfRangeException>(() => { int removed = sut.RemoveAt(-4); });
        }

        [Test]
        public void ShouldClear()
        {
            var sut = new ArrayList<int> { 1, 2, 3 };

            sut.Clear();
            Assert.AreEqual(0, sut.Count, "Should reset Count after Clear");
        }

        private static ArrayList<T> Create<T>()
        {
            return new ArrayList<T>();
        }
    }
}
