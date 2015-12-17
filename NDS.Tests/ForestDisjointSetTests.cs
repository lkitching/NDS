using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class ForestDisjointSetTests
    {
        [Test]
        public void Should_Make_Set()
        {
            var set = new ForestDisjointSet<int>();
            var items = TestGen.NRandomInts(1000, 2000).Distinct().ToArray();
            int e = items.First();

            MakeSets(set, items);

            TestAssert.IsSome(set.FindRepresentative(e), e);
        }

        [Test]
        public void Should_Not_Find_Representative()
        {
            var set = new ForestDisjointSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var rep = set.FindRepresentative("foo");

            TestAssert.IsNone(rep);
        }

        [Test]
        public void Should_Have_Different_Representatives()
        {
            var set = new ForestDisjointSet<int>();
            var numBatches = 4;
            var total = 10000;
            var items = TestGen.NRandomInts(total, total).Distinct().ToArray();
            int batchSize = items.Length / numBatches;

            MakeSets(set, items);

            var batches = Enumerable.Range(0, numBatches)
                .Select(b => items.Skip(b * batchSize).Take(batchSize).ToArray())
                .ToArray();

            foreach (var batch in batches)
            {
                for (int i = 0; i < batch.Length - 1; ++i)
                {
                    set.Merge(batch[i], batch[i + 1]);
                }
            }

            var reps = items.Select(set.FindRepresentative).Distinct().ToArray();
            Assert.AreEqual(numBatches, reps.Length, "Expected {0} disjoint sets", reps);
        }

        [Test]
        public void Should_Merge_Sets()
        {
            var set = new ForestDisjointSet<int>();
            var items = TestGen.NRandomInts(1000, 2000).Distinct().ToArray();
            MakeSets(set, items);

            int toMerge = 500;
            for (int i = 0; i < toMerge - 1; ++i)
            {
                set.Merge(items[i], items[i + 1]);
            }

            var reps = items.Take(toMerge).Select(i => set.FindRepresentative(i)).Distinct().ToArray();
            Assert.AreEqual(1, reps.Length, "All merged elements should have the same representative");

            Maybe<int> rep = reps[0];
            Assert.IsTrue(rep.HasValue, "Representative should be found");

            CollectionAssert.Contains(items.Take(toMerge), rep.Value, "Representative should exist within the merged elements");
        }

        private static void MakeSets<T>(ForestDisjointSet<T> set, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                set.MakeSet(item);
            }
        }
    }
}
