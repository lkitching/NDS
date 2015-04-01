using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class RandomisedBinarySearchTreeTests : MapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IEqualityComparer<TKey> keyComparer)
        {
            return new RandomisedBinarySearchTree<TKey, TValue>();
        }
    }
}
