using System.Collections.Generic;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class RedBlackTreeTests : OrderedMapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IComparer<TKey> keyComparer)
        {
            return new RedBlackTree<TKey, TValue>(keyComparer);
        }
    }
}
