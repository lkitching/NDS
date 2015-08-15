using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class BinomialQueueTests : PriorityQueueTests
    {
        protected override IPriorityQueue<T> Create<T>(IComparer<T> comparer)
        {
            return new BinomialHeap<T>(comparer);
        }

        [Test]
        public void Should_Merge_Queues()
        {
            var gen = new Random();
            var firstItems = TestGen.Generate(gen, r => r.Next(1, 10000)).Take(1000).ToArray();
            var secondItems = TestGen.Generate(gen, r => r.Next(15000, 25000)).Take(1000).ToArray();

            var sut = new BinomialHeap<int>();
            foreach (int i in firstItems) { sut.Insert(i); }

            var other = new BinomialHeap<int>();
            foreach(int i in secondItems) { other.Insert(i); }

            sut.Merge(other);

            var expectedItems = firstItems.Concat(secondItems).OrderBy(i => i).ToArray();

            CollectionAssert.AreEqual(expectedItems, Consume(sut));
        }
    }
}
