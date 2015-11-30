using NDS.Algorithms.Searching;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests.Algorithms.Searching
{
    public class BinarySearchTests
    {
        [Test]
        public void Empty_Range_Test()
        {
            int startIndex = 45;
            int[] items = TestGen.NRandomInts(50, 100).ToArray();
            var result = BinarySearch.Search(items, startIndex, startIndex - 1, 99, Comparer<int>.Default);

            Assert.IsFalse(result.Found, "Should not find item in empty range");
            Assert.AreEqual(45, result.Index);
        }

        [Test]
        public void Empty_Collection_Test()
        {
            int[] items = new int[0];
            var result = BinarySearch.Search(items, 0, -1, 6, Comparer<int>.Default);

            Assert.IsFalse(result.Found, "Should not find item in empty collection");
            Assert.AreEqual(0, result.Index);
        }

        [Test]
        public void Not_Found_Test()
        {
            int[] items = new[] { 1, 4, 7, 9 };
            var result = BinarySearch.Search(items, 0, items.Length - 1, 8, Comparer<int>.Default);

            Assert.IsFalse(result.Found, "Should not find item");
            Assert.AreEqual(3, result.Index, "Unexpected index");
        }

        [Test]
        public void Should_Find_Item()
        {
            var items = TestGen.NRandomInts(1000, 2000).Distinct().OrderBy(i => i).ToArray();
            int indexToFind = new Random().Next(0, items.Length);
            int item = items[indexToFind];

            var result = BinarySearch.Search(items, 0, items.Length - 1, item, Comparer<int>.Default);

            Assert.IsTrue(result.Found, "Should find item");
            Assert.AreEqual(indexToFind, result.Index, "Unexpected index");
        }

        [Test]
        public void Should_Find_Item_In_Range()
        {
            int item = 2;
            int[] items = new[] { 1, 2, 2, 5, 7 };
            var result = BinarySearch.Search(items, 2, 4, item, Comparer<int>.Default);

            Assert.IsTrue(result.Found, "Should have found item");
            Assert.AreEqual(2, result.Index);
        }
    }
}
