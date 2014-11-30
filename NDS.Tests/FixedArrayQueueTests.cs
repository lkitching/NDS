using System;
using NUnit.Framework;

namespace NDS 
{
    /// <summary>Tests for <see cref="FixedArrayQueue{T}"/>.</summary>
    [TestFixture]
    public class FixedArrayQueueTests : QueueTests
    {
        /// <summary>Tests the queue indicates when it is full.</summary>
        [Test]
        public void ShouldNotifyQueueFull()
        {
            var sut = CreateFullQueue(10);
            Assert.IsTrue(sut.IsFull);
        }

        /// <summary>Tests an exception is thrown when trying to enqueue an item into a full queue.</summary>
        [Test]
        public void EnqueueShouldThrowIfQueueFull()
        {
            var sut = CreateFullQueue(10);
            Assert.Throws<InvalidOperationException>(() => { sut.Enqueue(1); });
        }

        private static FixedArrayQueue<int> CreateFullQueue(int capacity)
        {
            var queue = new FixedArrayQueue<int>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                queue.Enqueue(i);
            }

            return queue;
        }

        protected override IQueue<T> Create<T>()
        {
            return new FixedArrayQueue<T>(100);
        }
    }
}
