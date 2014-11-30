using System;

namespace NDS
{
    /// <summary>Tests for <see cref="DynamicLinkedQueue{T}"/>.</summary>
    [NUnit.Framework.TestFixture]
    public class DynamicLinkedQueueTests : QueueTests
    {
        protected override IQueue<T> Create<T>()
        {
            return new DynamicLinkedQueue<T>();
        }
    }
}
