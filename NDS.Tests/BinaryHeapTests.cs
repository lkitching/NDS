using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using System.Diagnostics;

namespace NDS.Tests
{
    [TestFixture]
    public class BinaryHeapTests : PriorityQueueTests
    {
        protected override IPriorityQueue<T> Create<T>(IComparer<T> comparer)
        {
            return new BinaryHeap<T>(comparer);
        }
    }
}
