using NUnit.Framework;
using System.Collections.Generic;

namespace NDS.Tests
{
    [TestFixture]
    public class RandomisedBinarySearchTreeTests : OrderedMapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IComparer<TKey> keyComparer)
        {
            return new RandomisedBinarySearchTree<TKey, TValue>(keyComparer);
        }
    }
}
