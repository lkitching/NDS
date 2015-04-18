using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class TwoThreeTreeTests : MapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IEqualityComparer<TKey> keyComparer)
        {
            return new TwoThreeTree<TKey, TValue>();
        }

        [Test]
        public void Should_Iterate_Keys_In_Order()
        {
            var map = new TwoThreeTree<int, string>();
            var pairs = TestGen.NRandomInts(5, 10).Distinct().Select(i => new KeyValuePair<int, string>(i, "value" + i)).ToArray();

            foreach (var kvp in pairs)
            {
                map.Add(kvp);
            }

            var orderedPairs = pairs.OrderBy(kvp => kvp.Key).ToArray();
            CollectionAssert.AreEqual(orderedPairs, map, "Map should enumerate pairs in ascending key order");
        }
    }
}
