using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NDS.Algorithms.Sorting;

namespace NDS.Tests.Algorithms.Sorting
{
    [TestFixture]
    public class SortingAlgorithmTests
    {
        [Test]
        [TestCase(typeof(SelectionSort), Description = "Selection sort")]
        [TestCase(typeof(InsertionSort), Description = "Insertion sort")]
        [TestCase(typeof(BubbleSort), Description = "Bubble sort")]
        [TestCase(typeof(ShellSort), Description = "Shell sort")]
        [TestCase(typeof(QuickSort), Description = "Quick sort")]
        [TestCase(typeof(MergeSort), Description = "Merge sort")]
        [TestCase(typeof(HeapSort), Description = "Heap sort")]
        public void Sort_Test(Type sortType)
        {
            var values = TestGen.NRandomInts(50, 100).ToArray();
            var sort = (IInPlaceSort)Activator.CreateInstance(sortType);
            sort.Sort(values, Comparer<int>.Default);

            CollectionAssert.AreEqual(values.OrderBy(i => i), values, "Values not sorted");
        }
    }
}
