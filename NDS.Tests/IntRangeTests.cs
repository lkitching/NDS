﻿using System;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class IntRangeTests
    {
        [Test]
        public void ShouldBeEmpty()
        {
            var range = Empty();
            Assert.IsTrue(range.IsEmpty, "Should be empty when start >= end");
        }

        [Test]
        public void ShouldNotBeEmpty()
        {
            var r = new Random();
            int start = r.Next(1000);
            int count = r.Next(100, 10000);

            Assert.IsFalse(new IntRange(start, start + count).IsEmpty, "Should not be empty when start < end");
        }

        [Test]
        public void ShouldGetNonEmptyCount()
        {
            var r = new Random();
            int start = r.Next(1000);
            int count = r.Next(100, 10000);

            Assert.AreEqual(new IntRange(start, start + count).Count, count);
        }

        [Test]
        public void ShouldGetEmptyCount()
        {
            var range = Empty();
            Assert.AreEqual(0, range.Count, "Empty range should have count = 0");
        }

        [Test]
        public void ShouldContainItemsWithinRange()
        {
            var range = NonEmpty();
            for(int i = range.Start; i < range.End; ++i)
            {
                Assert.IsTrue(range.Contains(i), "Should contain item within range");
            }
        }

        [Test]
        public void ShouldNotContainEnd()
        {
            var range = NonEmpty();
            Assert.IsFalse(range.Contains(range.End), "Should not contain end element");
        }

        [Test]
        public void ShouldNotContainItemBeforeStart()
        {
            var range = NonEmpty();
            Assert.IsFalse(range.Contains(range.Start - 1));
        }

        [Test]
        public void IndexerShouldThrowIfIndexNegative()
        {
            var range = NonEmpty();
            Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[-1]; });
        }

        [Test]
        public void IndexerShouldThrowIfIndexTooLarge()
        {
            var range = NonEmpty();
            Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[range.Count]; });
        }

        [Test]
        public void ShouldEnumerate()
        {
            var range = NonEmpty();
            CollectionAssert.AreEqual(range, Enumerable.Range(range.Start, range.Count));
        }

        [Test]
        public void ShouldDropSome()
        {
            var range = NonEmpty();
            int toDrop = new Random().Next(1, range.Count);
            var dropped = range.Drop(toDrop);

            int expectedCount = range.Count - toDrop;

            Assert.AreEqual(expectedCount, dropped.Count, "Unexepcted count for dropped range");
            CollectionAssert.AreEqual(Enumerable.Range(range.End - expectedCount, expectedCount), dropped);
        }

        [Test]
        public void ShouldDropAll()
        {
            var range = NonEmpty();
            var dropped = range.Drop(range.Count);

            Assert.IsTrue(dropped.IsEmpty, "Should drop all items");
        }

        [Test]
        public void ShouldTake()
        {
            var range = NonEmpty();
            int toTake = new Random().Next(1, range.Count);
            var taken = range.Take(toTake);

            CollectionAssert.AreEqual(Enumerable.Range(range.Start, toTake), taken);
        }

        [Test]
        public void EmptyRangeMidpointShouldBeStart()
        {
            var r = new Random();
            int start = r.Next(1000, 10000);
            int diff = r.Next(500);

            Assert.AreEqual(start, IntRange.RangeMidpoint(start, start - diff));
        }

        [Test]
        public void MidpointNonEmptyEvenCount()
        {
            var r = new Random();
            int start = r.Next(1000, 10000);
            int n = r.Next(100, 1000);
            int count = n * 2;

            Assert.AreEqual(start + n - 1, IntRange.RangeMidpoint(start, start + count));
        }

        [Test]
        public void MidpointNonEmptyOddCount()
        {
            var r = new Random();
            int start = r.Next(1000, 10000);
            int n = r.Next(1, 1000);
            int count = n * 2 + 1;

            Assert.AreEqual(start + n, IntRange.RangeMidpoint(start, start + count));
        }

        [Test]
        public void RelationshipOf_Range_Before_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(1, 3));
            Assert.AreEqual(RangeRelationship.Before, rel);
        }

        [Test]
        public void RelationshipOf_OverlapsStart_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(4, 7));
            Assert.AreEqual(RangeRelationship.OverlapsStart, rel);
        }

        [Test]
        public void RelationshipOf_Equal_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(5, 10));
            Assert.AreEqual(RangeRelationship.Equal, rel);
        }

        [Test]
        public void RelationshipOf_Within_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(6, 9));
            Assert.AreEqual(RangeRelationship.Within, rel);
        }

        [Test]
        public void RelationshipOf_OverlapsEnd_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(7, 12));
            Assert.AreEqual(RangeRelationship.OverlapsEnd, rel);
        }

        [Test]
        public void RelationshipOf_After_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(10, 12));
            Assert.AreEqual(RangeRelationship.After, rel);
        }

        [Test]
        public void RelationshipOf_Encloses_Test()
        {
            var rel = new IntRange(5, 10).GetRelationshipOf(new IntRange(4, 11));
            Assert.AreEqual(RangeRelationship.Encloses, rel);
        }

        private static IntRange Empty()
        {
            var r = new Random();
            int start = r.Next(100, 1000);
            return new IntRange(start, start);
        }

        private static IntRange NonEmpty()
        {
            var r = new Random();
            int start = r.Next(1000);
            int count = r.Next(100, 10000);
            return new IntRange(start, start + count);
        }
    }
}
