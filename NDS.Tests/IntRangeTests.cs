using System;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class IntRangeTests
    {
        [Test]
        public void ShouldBeEmpty()
        {
            var range = Empty();
            Assert.IsTrue(range.IsEmpty, "Should be empty when start >= end");
        }

        [Test]
        public void ShouldNotBeEmpty()
        {
            var r = new Random();
            int start = r.Next(1000);
            int count = r.Next(100, 10000);

            Assert.IsFalse(new IntRange(start, start + count).IsEmpty, "Should not be empty when start < end");
        }

        [Test]
        public void ShouldGetNonEmptyCount()
        {
            var r = new Random();
            int start = r.Next(1000);
            int count = r.Next(100, 10000);

            Assert.AreEqual(new IntRange(start, start + count).Count, count);
        }

        [Test]
        public void ShouldGetEmptyCount()
        {
            var range = Empty();
            Assert.AreEqual(0, range.Count, "Empty range should have count = 0");
        }

        [Test]
        public void ShouldContainItemsWithinRange()
        {
            var range = NonEmpty();
            for(int i = range.Start; i < range.End; ++i)
            {
                Assert.IsTrue(range.Contains(i), "Should contain item within range");
            }
        }

        [Test]
        public void ShouldNotContainEnd()
        {
            var range = NonEmpty();
            Assert.IsFalse(range.Contains(range.End), "Should not contain end element");
        }

        [Test]
        public void ShouldNotContainItemBeforeStart()
        {
            var range = NonEmpty();
            Assert.IsFalse(range.Contains(range.Start - 1));
        }

        [Test]
        public void IndexerShouldThrowIfIndexNegative()
        {
            var range = NonEmpty();
            Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[-1]; });
        }

        [Test]
        public void IndexerShouldThrowIfIndexTooLarge()
        {
            var range = NonEmpty();
            Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[range.Count]; });
        }

        [Test]
        public void ShouldEnumerate()
        {
            var range = NonEmpty();
            CollectionAssert.AreEqual(range, Enumerable.Range(range.Start, range.Count));
        }

        [Test]
        public void ShouldDropSome()
        {
            var range = NonEmpty();
            int toDrop = new Random().Next(1, range.Count);
            var dropped = range.Drop(toDrop);

            int expectedCount = range.Count - toDrop;

            Assert.AreEqual(expectedCount, dropped.Count, "Unexepcted count for dropped range");
            CollectionAssert.AreEqual(Enumerable.Range(range.End - expectedCount, expectedCount), dropped);
        }

        [Test]
        public void ShouldDropAll()
        {
            var range = NonEmpty();
            var dropped = range.Drop(range.Count);

            Assert.IsTrue(dropped.IsEmpty, "Should drop all items");
        }

        [Test]
        public void ShouldTake()
        {
            var range = NonEmpty();
            int toTake = new Random().Next(1, range.Count);
            var taken = range.Take(toTake);

            CollectionAssert.AreEqual(Enumerable.Range(range.Start, toTake), taken);
        }

        private static IntRange Empty()
        {
            var r = new Random();
            int start = r.Next(100, 1000);
            int diff = r.Next(10);
            return new IntRange(start, start - diff);
        }

        private static IntRange NonEmpty()
        {
            var r = new Random();
            int start = r.Next(1000);
            int count = r.Next(100, 10000);
            return new IntRange(start, start + count);
        }
    }
}
