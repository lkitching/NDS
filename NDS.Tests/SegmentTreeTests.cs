using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using NUnit.Framework;
using FsCheck;

namespace NDS.Tests
{
    [TestFixture]
    public class SegmentTreeTests
    {
        [Test]
        public void Should_Create_Empty_Test()
        {
            var arr = new int[0];
            var sut = new SegmentTree<int>(arr, Math.Min);
        }

        [Test]
        public void Find_Range_Test()
        {
            var gen = from arr in TestGen.NonEmptyArrayOf(Arb.Default.Int32().Generator)
                      from start in Gen.Choose(0, arr.Length - 1)
                      from end in Gen.Choose(start + 1, arr.Length)
                      select new { Source = arr, Range = new IntRange(start, end) };

            var data = Gen.Sample(200, 1, gen).Head;
            Func<int, int, int> f = Math.Min;
            var sut = new SegmentTree<int>(data.Source, f);
            var expected = CalculateRange(data.Source, data.Range, f);
            var actual = sut.FindRange(data.Range);

            Assert.AreEqual(expected, actual, "Range results differ");
        }

        [Test]
        public void Updated_Test()
        {
            var intGen = Arb.Default.Int32().Generator;
            var gen = from arr in TestGen.NonEmptyArrayOf(intGen)
                      from start in Gen.Choose(0, arr.Length - 1)
                      from end in Gen.Choose(start + 1, arr.Length)
                      from updatedIndex in Gen.Choose(start, Math.Min(end, arr.Length - 1))
                      from updatedValue in intGen
                      select new {
                          Source = arr,
                          Range = new IntRange(start, end),
                          UpdatedIndex = updatedIndex,
                          UpdatedValue = updatedValue
                      };

            var data = Gen.Sample(200, 1, gen).Head;
            Func<int, int, int> f = Math.Min;

            var sut = new SegmentTree<int>(data.Source, f);
            data.Source[data.UpdatedIndex] = data.UpdatedValue;
            sut.Updated(data.UpdatedIndex);

            var expected = CalculateRange(data.Source, data.Range, f);
            var actual = sut.FindRange(data.Range);

            Assert.AreEqual(expected, actual, "Range results differ");
        }

        private static T CalculateRange<T>(IReadOnlyList<T> source, IntRange range, Func<T, T, T> f)
        {
            Debug.Assert(!range.IsEmpty, "Range should not be empty");

            T v = source[range.Start];
            for(int i = range.Start + 1; i < range.End; ++i)
            {
                v = f(v, source[i]);
            }
            return v;
        }
    }
}
