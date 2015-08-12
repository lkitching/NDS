using NUnit.Framework;
using System.Collections.Generic;

namespace NDS.Tests
{
    [TestFixture(Description = "Splay tree tests")]
    public class SplayTreeTests : OrderedMapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IComparer<TKey> keyComparer)
        {
            return new SplayTree<TKey, TValue>(keyComparer);
        }
    }
}
