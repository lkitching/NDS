using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class ClosedAddressingHashTableTests : MapTests
    {
        [Test]
        public void Should_Resize_Table()
        {
            int initialCapacity = 10;
            double maxLoadFactor = 2;
            var table = CreateTable<int, string>(EqualityComparer<int>.Default, initialCapacity, maxLoadFactor);

            int untilResize = (int)Math.Floor(initialCapacity * maxLoadFactor);
            PopulateWith(table, Enumerable.Range(1, untilResize));

            Assert.AreEqual(initialCapacity, table.Capacity, "Unexpected capacity in table");

            table.Add(untilResize + 1, "straw");
            Assert.Greater(table.Capacity, initialCapacity, "Failed to increase capacity after max load size reached");
            Assert.Less(table.LoadFactor, maxLoadFactor, "Failed to calculate load factor after resize");
        }

        private ClosedAddressingHashTable<TKey, TValue> CreateTable<TKey, TValue>(IEqualityComparer<TKey> keyComparer, int capacity, double maxLoadFactor)
        {
            return new ClosedAddressingHashTable<TKey, TValue>(keyComparer, capacity, maxLoadFactor);
        }

        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IEqualityComparer<TKey> keyComparer)
        {
            return new ClosedAddressingHashTable<TKey, TValue>(keyComparer);
        }
    }
}
