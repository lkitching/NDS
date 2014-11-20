using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using System.Diagnostics;

namespace NDS 
{
    [TestFixture]
    public class BinaryHeapTests
    {
        [Test]
        public void ShouldBeEmptyInitially()
        {
            var sut = Create<int>();
            Assert.AreEqual(0, sut.Count);
        }

        [Test]
        public void GetMinShouldThrowForEmptyHeap()
        {
            var sut = Create<int>();
            Assert.Throws<InvalidOperationException>(() => { int min = sut.Min; });
        }

        [Test]
        public void ShouldIncrementCountAfterInsert()
        {
            var sut = Create<int>();
            sut.Insert(1);

            Assert.AreEqual(1, sut.Count, "Failed to increment count on insert");
        }

        [Test]
        public void ShouldGetMin()
        {
            var sut = Create<int>();
            sut.Insert(4);
            sut.Insert(2);
            sut.Insert(1);

            AssertMin(1, sut);
        }

        [Test]
        public void ShouldRemoveMin()
        {
            var sut = Create<int>();
            sut.Insert(4);
            sut.Insert(2);
            sut.Insert(1);

            int min = sut.RemoveMin();
            AssertMin(1, min);
        }

        [Test]
        public void RemoveMinShouldDecrementCount()
        {
            var sut = Create<int>();
            sut.Insert(4);
            sut.Insert(2);
            sut.Insert(1);

            int min = sut.RemoveMin();
            Assert.AreEqual(2, sut.Count, "Failed to decrement count on minimum");
        }

        [Test]
        public void ShouldUseCustomComparer()
        {
            var comparer = new CustomComparer();
            var sut = new BinaryHeap<Custom>(comparer);

            Random r = new Random();
            var items = Enumerable.Repeat(1, 10000).Select(_ => new Custom(r.Next())).ToArray();

            int minValue = items.Select(c => c.Value).Min();

            foreach (Custom item in items)
            {
                sut.Insert(item);
            }

            AssertMin(minValue, sut.Min.Value);
        }

        [Test]
        public void ShouldAllowNull()
        {
            var sut = new BinaryHeap<Custom>(new CustomComparer());
            sut.Insert(new Custom(3));
            sut.Insert(null);

            Assert.IsNull(sut.Min);
        }

        [Test]
        public void ShouldInsertItems()
        {
            Random r = new Random();
            var items = Enumerable.Repeat(1, 10000).Select(_ => r.Next()).ToArray();

            var sut = Create<int>();

            foreach (int i in items)
            {
                sut.Insert(i);
            }

            int minItem = items.Min();
            AssertMin(minItem, sut);
        }

        [Test]
        public void ShouldAllowDuplicates()
        {
            var sut = Create<int>();
            sut.Insert(4);
            sut.Insert(4);

            Assert.AreEqual(2, sut.Count, "Unexpected count");
            AssertMin(4, sut);
        }

        private static BinaryHeap<T> Create<T>()
        {
            return new BinaryHeap<T>();
        }

        private class Custom
        {
            public Custom(int i)
            {
                this.Value = i;
            }

            public int Value { get; private set; }
        }

        private class CustomComparer : IComparer<Custom>
        {
            public int Compare(Custom x, Custom y)
            {
                if (x == null) return y == null ? 0 : -1;
                if (y == null) return 1;

                Debug.Assert(x != null);
                Debug.Assert(y != null);

                return x.Value.CompareTo(y.Value);
            }
        }

        private static void AssertMin<T>(T expected, T actual)
        {
            Assert.AreEqual(expected, actual, "Unexpected minimum value");
        }

        private static void AssertMin<T>(T expected, BinaryHeap<T> heap)
        {
            AssertMin(expected, heap.Min);
        }
    }
}
