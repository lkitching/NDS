using System;
using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="DynamicStack{T}"/>.</summary>
    [TestFixture]
    public class DynamicStackTests : StackTests
    {
        protected override IStack<T> Create<T>()
        {
            return new DynamicStack<T>();
        }
    }
}
