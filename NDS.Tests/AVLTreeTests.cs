using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class AVLTreeTests : OrderedMapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>(IComparer<TKey> keyComparer)
        {
            return new AVLTree<TKey, TValue>(keyComparer);
        }
    }
}
