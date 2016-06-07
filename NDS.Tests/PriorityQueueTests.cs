using System;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using System.Diagnostics;

namespace NDS.Tests
{
    public abstract class PriorityQueueTests
    {
        [Test]
        public void Should_Be_Initially_Empty()
        {
            var sut = Create<int>();
            Assert.AreEqual(0, sut.Count);
        }

        [Test]
        public void Get_Minimum_Should_Throw_For_Empty_Heap()
        {
            var sut = Create<int>();
            Assert.Throws<InvalidOperationException>(() => { int min = sut.GetMinimum(); });
        }

        [Test]
        public void Should_Increment_Count_After_Insert()
        {
            var sut = Create<int>();
            sut.Insert(1);

            Assert.AreEqual(1, sut.Count, "Failed to increment count on insert");
        }

        [Test]
        public void Should_Insert_All()
        {
            var items = TestGen.NRandomInts(100, 200).ToArray();
            var sut = Create<int>();
            sut.InsertAll(items);

            Assert.AreEqual(items.Length, sut.Count, "Unexpected count");
        }

        [Test]
        public void Should_Get_Minimum()
        {
            var sut = Create<int>();
            var items = TestGen.NRandomInts(1000, 5000).ToArray();
            sut.InsertAll(items);

            var min = items.Min();

            AssertMin(min, sut);
        }

        [Test]
        public void Should_Remove_Minimum()
        {
            var sut = Create<int>();
            var items = TestGen.NRandomInts(1000, 5000).ToArray();
            sut.InsertAll(items);

            int removed = sut.RemoveMinimum();
            AssertMin(items.Min(), removed);
        }

        [Test]
        public void Should_Remove_All_In_Order()
        {
            var sut = Create<int>();
            var items = TestGen.NRandomInts(10000, 20000).ToArray();
            sut.InsertAll(items);

            var minimums = Consume(sut).ToArray();
            CollectionAssert.AreEqual(items.OrderBy(i => i), minimums, "Failed to remove all items in order");
        }

        [Test]
        public void Remove_Minimum_Should_Decrement_Count()
        {
            var sut = Create<int>();
            sut.Insert(4);
            sut.Insert(2);
            sut.Insert(1);

            int min = sut.RemoveMinimum();
            Assert.AreEqual(2, sut.Count, "Failed to decrement count on minimum");
        }

        [Test]
        public void Remove_Minimum_Should_Throw_On_Empty_Queue()
        {
            var sut = Create<int>();
            Assert.Throws<InvalidOperationException>(() => { sut.RemoveMinimum(); });
        }

        [Test]
        public void Should_Use_Custom_Comparer()
        {
            var comparer = new CustomComparer();
            var sut = new BinaryHeap<Custom>(comparer);

            Random r = new Random();
            var items = Enumerable.Repeat(1, 10000).Select(_ => new Custom(r.Next())).ToArray();

            int minValue = items.Select(c => c.Value).Min();

            sut.InsertAll(items);

            AssertMin(minValue, sut.Min.Value);
        }

        [Test]
        public void Should_Allow_Null()
        {
            var sut = new BinaryHeap<Custom>(new CustomComparer());
            sut.Insert(new Custom(3));
            sut.Insert(null);

            Assert.IsNull(sut.Min);
        }

        [Test]
        public void Should_Allow_Duplicates()
        {
            var sut = Create<int>();
            sut.Insert(4);
            sut.Insert(4);

            Assert.AreEqual(2, sut.Count, "Unexpected count");
            AssertMin(4, sut);
        }

        [Test]
        public void Clear_Should_Reset_Count()
        {
            var sut = CreateAndPopulate();
            sut.Clear();
            Assert.AreEqual(0, sut.Count, "Count should be 0 after Clear()");
        }

        [Test]
        public void Get_Minimum_Should_Fail_After_Clear()
        {
            var sut = CreateAndPopulate();
            sut.Clear();
            Assert.Throws<InvalidOperationException>(() => { sut.GetMinimum(); });
        }

        [Test]
        public void Remove_Minimum_Should_Fail_After_Clear()
        {
            var sut = CreateAndPopulate();
            sut.Clear();
            Assert.Throws<InvalidOperationException>(() => { sut.RemoveMinimum(); });
        }

        private IPriorityQueue<T> Create<T>()
        {
            return Create(Comparer<T>.Default);
        }

        private IPriorityQueue<int> CreateAndPopulate()
        {
            var queue = Create<int>();
            foreach (var i in TestGen.NRandomInts(1000, 5000))
            {
                queue.Insert(i);
            }
            return queue;
        }

        protected abstract IPriorityQueue<T> Create<T>(IComparer<T> comparer);

        protected static IEnumerable<T> Consume<T>(IPriorityQueue<T> queue)
        {
            while (queue.Count > 0)
            {
                yield return queue.RemoveMinimum();
            }
        }

        private static void AssertMin<T>(T expected, T actual)
        {
            Assert.AreEqual(expected, actual, "Unexpected minimum value");
        }

        private static void AssertMin<T>(T expected, IPriorityQueue<T> queue)
        {
            AssertMin(expected, queue.GetMinimum());
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
    }
}
