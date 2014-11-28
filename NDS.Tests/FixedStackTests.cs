using System;
using System.Linq;
using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="FixedStack{T}"/>.</summary>
    [TestFixture]
    public class FixedStackTests : StackTests
    {
        /// <summary>Tests an exception is thrown when trying to push onto a full stack.</summary>
        [Test]
        public void PushShouldThrowWhenAtCapacity()
        {
            var r = new Random();
            int count = r.Next(20, 100);
            var stack = new FixedStack<int>(count);

            foreach (int i in Enumerable.Range(1, count))
            {
                stack.Push(i);
            }

            Assert.Throws<InvalidOperationException>(() => { stack.Push(r.Next()); });
        }

        protected override IStack<T> Create<T>()
        {
            return new FixedStack<T>(100);
        }
    }
}
