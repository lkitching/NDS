using System;
using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for all <see cref="IStack{T}"/> implementations.</summary>
    public abstract class StackTests
    {
        protected abstract IStack<T> Create<T>();

        /// <summary>Tests the initial count should be 0.</summary>
        [Test]
        public void CountShouldBeZeroInitially()
        {
            var sut = this.Create<int>();
            Assert.AreEqual(0, sut.Count);
        }

        /// <summary>Tests an exception is thrown when trying to pop from an empty stack.</summary>
        [Test]
        public void ShouldNotPopEmptyStack()
        {
            var sut = this.Create<int>();
            Assert.Throws<InvalidOperationException>(() => { var _ = sut.Pop(); });
        }

        /// <summary>Tests an exception is thrown when trying to peek from an empty stack.</summary>
        [Test]
        public void ShouldNotPeekEmptyStack()
        {
            var sut = this.Create<int>();
            Assert.Throws<InvalidOperationException>(() => { int item = sut.Peek(); });
        }

        /// <summary>Tests an item can be pushed.</summary>
        [Test]
        public void ShouldPushItem()
        {
            int item = 1;
            var sut = this.Create<int>();

            sut.Push(item);
            Assert.AreEqual(item, sut.Peek());
        }

        /// <summary>Tests the count is incremented when pushing an item.</summary>
        [Test]
        public void PushShouldIncrementCount()
        {
            var sut = this.Create<int>();
            sut.Push(4);
            Assert.AreEqual(1, sut.Count);
        }

        /// <summary>Tests an item can be popped.</summary>
        [Test]
        public void ShouldPopItem()
        {
            int item = 1;
            var sut = this.Create<int>();

            sut.Push(item);
            int removed = sut.Pop();

            Assert.AreEqual(item, removed);
        }

        /// <summary>Tests popping an item decrements the count.</summary>
        [Test]
        public void PopShouldDecrementCount()
        {
            var sut = this.Create<int>();
            sut.Push(1);
            sut.Pop();

            Assert.AreEqual(0, sut.Count);
        }
    }
}
