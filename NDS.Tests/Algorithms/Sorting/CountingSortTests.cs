using System;
using System.Linq;
using NUnit.Framework;

using NDS.Algorithms.Sorting;

namespace NDS.Tests.Algorithms.Sorting
{
    [TestFixture]
    public class CountingSortTests
    {
        [Test]
        public void ShouldSort()
        {
            var items = new[] { "aa", "bbbb", "c", "d", "ee", "fffffff", "gg", "hhhhh" };
            var sort = new CountingSort();
            Func<string, int> keyFunc = s => s.Length;
            sort.Sort(items, keyFunc);

            //NOTE: Enumerable.OrderBy is stable sort as counting sort should be
            CollectionAssert.AreEqual(items.OrderBy(keyFunc), items);
        }
    }
}
