using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class VEBTreeTests
    {
        [Test]
        public void Should_Be_Initially_Empty()
        {
            var veb = new VEBTree();
            Assert.IsTrue(veb.IsEmpty, "Should be empty initially");
        }

        [Test]
        public void Should_Not_Contain_Value()
        {
            var veb = new VEBTree();
            Assert.IsFalse(veb.Contains(4), "Should not contain value initially");
        }

        [Test]
        public void Should_Contain_Value_After_Insert()
        {
            var values = TestGen.NRandomInts(5000, 10000).Where(i => i >= 0).Select(i => (uint)i).ToArray();
            var veb = new VEBTree();
            veb.InsertAll(values);

            foreach (var value in values)
            {
                Assert.IsTrue(veb.Contains(value), "Should contain value after insert");
            }
        }

        [Test]
        public void Should_Set_Min_And_Max()
        {
            var values = TestGen.NRandomInts(5000, 10000).Where(i => i >= 0).Select(i => (uint)i).ToArray();
            var veb = new VEBTree();
            veb.InsertAll(values);

            Assert.AreEqual(Maybe.Some(values.Min()), veb.FindMin(), "Unexpected minimum");
            Assert.AreEqual(Maybe.Some(values.Max()), veb.FindMax(), "Unexpected maximum");
        }

        [Test]
        public void Should_Find_Next()
        {
            var values = RandomUInts();
            var veb = new VEBTree();
            veb.InsertAll(values);

            TestAssert.IsNone(veb.FindNext(values.Max()), "Should not find next for max value");

            var orderedValues = values.OrderBy(i => i).ToArray();
            for (int i = 0; i < orderedValues.Length - 1; ++i)
            {
                var expectedNext = Maybe.Some(orderedValues[i + 1]);
                var next = veb.FindNext(orderedValues[i]);
                Assert.AreEqual(expectedNext, next, "Unexpected next value");
            }
        }

        [Test]
        public void Should_Find_Previous()
        {
            var values = RandomUInts();
            var veb = new VEBTree();
            veb.InsertAll(values);

            TestAssert.IsNone(veb.FindPrevious(values.Min()), "Should not find previous for min value");

            var orderedValues = values.OrderByDescending(i => i).ToArray();
            for (int i = 0; i < orderedValues.Length - 1; ++i)
            {
                var expectedPrevious = Maybe.Some(orderedValues[i + 1]);
                var previous = veb.FindPrevious(orderedValues[i]);
                Assert.AreEqual(expectedPrevious, previous, "Unexpected previous value");
            }
        }

        [Test]
        public void Should_Remove_Values()
        {
            var values = RandomUInts();
            var veb = new VEBTree();
            veb.InsertAll(values);

            foreach (var value in values)
            {
                bool removed = veb.Delete(value);
                Assert.IsTrue(removed, "Failed to remove value");

                bool contains = veb.Contains(value);
                Assert.IsFalse(contains, "Tree contains value after removal");
            }
        }

        [Test]
        public void Should_Fail_To_Remove_NonExistent_Values()
        {
            var values = RandomUInts();
            var veb = new VEBTree();
            veb.InsertAll(values);

            foreach (uint missing in RandomUInts().Except(values))
            {
                bool removed = veb.Delete(missing);
                Assert.IsFalse(removed, "Removed non-existent value");
            }
        }

        [Test]
        public void Remove_Should_Update_Min()
        {
            var values = RandomUInts();
            var veb = new VEBTree();
            veb.InsertAll(values);

            var orderedValues = values.OrderBy(i => i).ToArray();
            for(int i = 0; i < orderedValues.Length - 1; ++i)
            {
                bool removed = veb.Delete(orderedValues[i]);
                var min = veb.FindMin();
                var expectedMin = Maybe.Some(orderedValues[i + 1]);
                Assert.AreEqual(expectedMin, min, "Unexpected minimum after removal");
            }

            //remove last item
            bool removedLast = veb.Delete(orderedValues.Last());
            Assert.IsTrue(removedLast, "Failed to remove last item");

            TestAssert.IsNone(veb.FindMin(), "Should be no minimum after all items have been removed");
        }

        [Test]
        public void Remove_Should_Update_Max()
        {
            var values = RandomUInts();
            var veb = new VEBTree();
            veb.InsertAll(values);

            var orderedValues = values.OrderBy(i => i).ToArray();
            for (int i = orderedValues.Length - 1; i >= 1; --i)
            {
                bool removed = veb.Delete(orderedValues[i]);
                var max = veb.FindMax();
                var expectedMax = Maybe.Some(orderedValues[i - 1]);
                Assert.AreEqual(expectedMax, max, "Unexpected maximum after removal");
            }

            bool removedLast = veb.Delete(orderedValues[0]);
            Assert.IsTrue(removedLast, "Failed to remove last item");

            TestAssert.IsNone(veb.FindMax(), "Should be no maximum after all items have been removed");
        }

        private static uint[] RandomUInts(int minCount = 5000, int maxCount = 10000)
        {
            return TestGen.NRandomInts(5000, 10000).Where(i => i >= 0).Select(i => (uint)i).ToArray();
        }
    }
}
