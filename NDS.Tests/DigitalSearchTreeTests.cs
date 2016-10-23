using System.Collections.Generic;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class DigitalSearchTreeTests : UnorderedMapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IEqualityComparer<TKey> keyComparer)
        {
            return (IMap<TKey, TValue>)new DigitalSearchTree<int, string>(BitAddressable.Int32);
        }
    }
}
