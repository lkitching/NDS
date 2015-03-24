using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class BinarySearchTreeTests : MapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IEqualityComparer<TKey> keyComparer)
        {
            return new BinarySearchTree<TKey, TValue>();
        }
    }
}
