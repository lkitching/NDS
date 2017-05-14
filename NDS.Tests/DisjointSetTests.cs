using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using FsCheck;

namespace NDS.Tests
{
    public abstract class DisjointSetTests
    {
        protected class IntSet
        {
            private readonly HashSet<int> otherItems = new HashSet<int>();

            public IntSet(IEnumerable<int> items)
            {
                this.Representative = items.First();
                this.otherItems = new HashSet<int>(items.Skip(1));
            }

            public int Representative { get; private set; }

            public IEnumerable<int> OtherItems
            {
                get { return this.otherItems; }
            }

            public IEnumerable<int> AllItems
            {
                get
                {
                    return new[] { this.Representative }.Concat(this.otherItems);
                }
            }
        }

        protected class ListDisjointSet
        {
            public ListDisjointSet(int count, List<IntSet> sets)
            {
                this.Count = count;
                this.Sets = sets;
            }

            public List<IntSet> Sets { get; private set; }
            public int Count { get; private set; }
        }

        private static Gen<ListDisjointSet> SetsGen()
        {
            return from count in Arb.Default.PositiveInt().Generator
                   from numSets in Gen.Choose(1, count.Get)
                   from items in Gen.ListOf(count.Get, Gen.Elements(Enumerable.Range(0, numSets)))
                   let groups = items.Select((grp, item) => new { Group = grp, Item = item }).GroupBy(i => i.Group)
                   let sets = groups.Select(grp => new IntSet(grp.Select(g => g.Item))).ToList()
                   select new ListDisjointSet(count.Get, sets);
        }

        protected abstract IDisjointSet<int> Create(ListDisjointSet sets);

        private static ListDisjointSet RandomSets()
        {
            return Gen.Sample(200, 1, SetsGen()).Head;
        }

        [Test]
        public void Should_Not_Find_Representative()
        {
            var source = RandomSets();
            var sets = Create(source);

            var rep = sets.FindRepresentative(source.Count);
            Assert.IsFalse(rep.HasValue, "Found representative for non-existent value");
        }

        [Test]
        public void Should_Have_Same_Representatives()
        {
            var source = RandomSets();
            var sut = Create(source);

            foreach(var set in source.Sets)
            {
                foreach(var item in set.OtherItems)
                {
                    Assert.AreEqual(sut.FindRepresentative(set.Representative), sut.FindRepresentative(item), "Representatives differ");
                }
            }
        }

        [Test]
        public void Representative_Should_Be_Member_Of_Set()
        {
            var source = RandomSets();
            var sut = Create(source);

            foreach(var set in source.Sets)
            {
                var rep = sut.FindRepresentative(set.Representative);
                Assert.IsTrue(rep.HasValue, "Failed to find representative");

                CollectionAssert.Contains(set.AllItems, rep.Value, "Representative value not member of set");
            }
        }

        [Test]
        public void Should_Have_Different_Representatives()
        {
            var source = RandomSets();
            var sut = Create(source);

            var reps = source.Sets.Select(s => sut.FindRepresentative(s.Representative)).GroupBy(i => i);
            Assert.AreEqual(source.Sets.Count, reps.Count(), "Sets should have unique representative");
        }

        [Test]
        public void Should_Merge_Sets()
        {
            var source = RandomSets();
            var sut = Create(source);

            var set1 = source.Sets[0];
            var set2 = source.Sets[new System.Random().Next(1, source.Sets.Count)];

            sut.Merge(set1.Representative, set2.Representative);

            var reps = set1.AllItems.Concat(set2.AllItems).Select(i => sut.FindRepresentative(i)).ToArray();

            foreach(var r in reps)
            {
                Assert.IsTrue(r.HasValue, "Representative not found");
            }

            var groups = reps.Select(r => r.Value).GroupBy(i => i);
            Assert.AreEqual(1, groups.Count(), "Merged sets should have the same representative");
        }
    }
}
