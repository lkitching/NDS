using NDS.Algorithms.Sorting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests.Algorithms.Sorting
{
    [TestFixture]
    public class RadixSortTests
    {
        [TestCase(typeof(LSDRadixSort))]
        [TestCase(typeof(MSDRadixSort))]
        public void ShouldRadixSort(Type sortType)
        {
            var items = TestGen.NRandomInts(100000, 200000).ToArray();
            var sort = (IRadixSort)(Activator.CreateInstance(sortType));
            sort.RadixSort(items, new Int32Radix(), 0, items.Length);

            Assert.IsTrue(Seq.IsSorted(items), "Failed to sort");
        }
    }
}
