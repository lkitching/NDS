using System;
using System.Linq;

using NUnit.Framework;

namespace NDS
{
    /// <summary>Tests common to all <see cref="IQueue{T}"/> implementations.</summary>
    public abstract class QueueTests
    {
        protected abstract IQueue<T> Create<T>();

        /// <summary>Tests the initial count is 0.</summary>
        [Test]
        public void InitialCountShouldBeZero()
        {
            var sut = this.Create<int>();
            Assert.AreEqual(0, sut.Count, "Initial count should be 0");
        }

        /// <summary>Tests the count is incremented after enqueuing an item.</summary>
        [Test]
        public void EnqueueShouldIncrementCount()
        {
            var sut = this.Create<int>();
            sut.Enqueue(1);

            Assert.AreEqual(1, sut.Count, "Enqueue should increment count");
        }

        /// <summary>Tests an item can be dequeued.</summary>
        [Test]
        public void ShouldDequeueItem()
        {
            var sut = this.Create<int>();
            sut.Enqueue(1);
            sut.Enqueue(2);
            sut.Enqueue(3);

            int first = sut.Dequeue();
            Assert.AreEqual(1, first, "Dequeued unexpected item");
        }

        /// <summary>Tests the count is decremented after a dequeue operation.</summary>
        [Test]
        public void DequeueShouldDecrementCount()
        {
            var sut = this.Create<int>();
            sut.Enqueue(1);
            sut.Enqueue(2);
            sut.Enqueue(3);

            Assert.AreEqual(3, sut.Count, "Unexpected count before dequeue");
            int ignored = sut.Dequeue();
            Assert.AreEqual(2, sut.Count, "Dequeue should decrement count");
        }

        /// <summary>Tests an exception is thrown when dequeuing from an empty queue.</summary>
        [Test]
        public void DequeueShouldThrowIfQueueEmpty()
        {
            var sut = this.Create<int>();
            Assert.Throws<InvalidOperationException>(() => { var _ = sut.Dequeue(); });
        }

        /// <summary>Tests the items in the queue can be enumerated.</summary>
        [Test]
        public void ShouldEnumerateItems()
        {
            var items = Enumerable.Range(10, 20).ToArray();
            var sut = this.Create<int>();

            sut.EnqueueInserter().InsertAll(items);

            CollectionAssert.AreEqual(items, sut, "Unexpected items in queue");
        }
    }
}
