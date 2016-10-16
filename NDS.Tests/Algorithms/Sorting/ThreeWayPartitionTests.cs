using System;
using System.Collections.Generic;

using NUnit.Framework;

using NDS.Algorithms.Sorting;

namespace NDS.Tests.Algorithms.Sorting
{
    [TestFixture]
    public class ThreeWayPartitionTests
    {
        [Test]
        public void Partition_Test()
        {
            var items = Create();
            var result = ThreeWayPartition.Partition(items, 0, items.Length, Comparer<int>.Default);

            int pivot = items[result.EqStartIndex];
            for(int i = 0; i < result.EqStartIndex; ++i)
            {
                Assert.Less(items[i], pivot, "Should be < pivot");
            }

            for(int i = result.EqStartIndex; i < result.GtStartIndex; ++i)
            {
                Assert.AreEqual(items[i], pivot, "Should = pivot");
            }

            for(int i = result.GtStartIndex; i < items.Length; ++i)
            {
                Assert.Greater(items[i], pivot, "Should be > pivot");
            }
        }

        private static int[] Create()
        {
            var r = new Random();
            int len = r.Next(10000, 20000);

            var items = new int[len];
            for(int i = 0; i < items.Length; ++i)
            {
                items[i] = r.Next(10);
            }

            return items;
        }
    }
}
