using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NDS.Tests
{
    [TestFixture]
    public class GapBufferTests
    {
        [Test]
        public void Constructor_Should_Set_Point()
        {
            var buf = new GapBuffer<int>();
            Assert.AreEqual(0, buf.Point);
        }

        [Test]
        public void Constructor_Should_Set_Capacity()
        {
            int capacity = new Random().Next(1, 50);
            var buf = new GapBuffer<int>(capacity);
            Assert.AreEqual(capacity, buf.Capacity, "Unexpected capacity");
        }

        [Test]
        public void Count_Should_Be_Zero_Initially()
        {
            var buf = new GapBuffer<int>();
            Assert.AreEqual(0, buf.Count, "Unexpected count");
        }

        [Test]
        public void Should_Insert_Into_Empty()
        {
            var items = TestGen.NRandomInts(1, 50).ToArray();
            var buf = new GapBuffer<int>();
            foreach(var i in items) { buf.Insert(i); }

            Assert.AreEqual(items.Length, buf.Count, "Unexpected count after insert");
            CollectionAssert.AreEqual(items, buf, "Unexpected items in buffer");
        }

        [Test]
        public void Should_Insert_At_Point()
        {
            var before = TestGen.NRandomInts(1, 5).ToArray();
            var after = TestGen.NRandomInts(1, 5).ToArray();
            var toInsert = new Random().Next();

            var buf = new GapBuffer<int>();
            foreach(var i in before.Concat(after)) { buf.Insert(i); }

            //move point and insert
            buf.Point = before.Length;
            buf.Insert(toInsert);

            var expected = before.Concat(new[] { toInsert }).Concat(after).ToArray();
            CollectionAssert.AreEqual(expected, buf, "Unexpected after insert at point");
        }

        [Test]
        public void Point_Setter_Should_Throw_When_Negative()
        {
            var buf = new GapBuffer<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { buf.Point = -1; });
        }

        [Test]
        public void Point_Setter_Should_Throw_When_Greater_Than_Count()
        {
            var items = TestGen.NRandomInts(5, 50).ToArray();
            var buf = new GapBuffer<int>();
            foreach(var i in items) { buf.Insert(i); }

            Assert.Throws<ArgumentOutOfRangeException>(() => { buf.Point = items.Length + 1; });
        }

        [Test]
        public void RemovePrevious_Should_Throw_On_Empty()
        {
            var buf = new GapBuffer<int>();
            Assert.Throws<InvalidOperationException>(() => { buf.RemovePrevious(); });
        }

        [Test]
        public void RemovePrevious_Should_Throw_If_Point_At_Start()
        {
            var buf = CreateNonEmpty();
            buf.Point = 0;
            Assert.Throws<InvalidOperationException>(() => { buf.RemovePrevious(); });
        }

        [Test]
        public void RemovePrevious_Test()
        {
            var items = TestGen.NRandomInts(10, 50).ToArray();
            int removeAt = new Random().Next(0, items.Length);

            var buf = new GapBuffer<int>();
            InsertAll(buf, items);

            buf.Point = removeAt + 1;
            var removed = buf.RemovePrevious();

            var expected = items[removeAt];
            Assert.AreEqual(expected, removed, "Unexpected removed item");

            //point should be moved back
            Assert.AreEqual(removeAt, buf.Point, "Unexpected point after removal");

            Assert.AreEqual(items.Length - 1, buf.Count, "Unexpected count after removal");
        }

        [Test]
        public void RemoveNext_Should_Throw_If_Point_At_End()
        {
            var buf = CreateNonEmpty();
            buf.Point = buf.Count;
            Assert.Throws<InvalidOperationException>(() => { buf.RemoveNext(); });
        }

        [Test]
        public void RemoveNext_Should_Throw_If_Empty()
        {
            var buf = new GapBuffer<int>();
            Assert.Throws<InvalidOperationException>(() => { buf.RemoveNext(); });
        }

        [Test]
        public void RemoveNext_Test()
        {
            var items = TestGen.NRandomInts(10, 50).ToArray();
            int removeAt = new Random().Next(0, items.Length);

            var buf = new GapBuffer<int>();
            InsertAll(buf, items);

            buf.Point = removeAt;
            int removed = buf.RemoveNext();

            int expected = items[removeAt];
            Assert.AreEqual(expected, removed, "Unexpected removed value");

            //point should be unchanged
            Assert.AreEqual(removeAt, buf.Point, "Unexpected point after removal");

            Assert.AreEqual(items.Length - 1, buf.Count, "Unexpected count after removal");
        }

        private static GapBuffer<int> CreateNonEmpty()
        {
            var items = TestGen.NRandomInts(10, 50).ToArray();
            var buf = new GapBuffer<int>(items.Length);
            InsertAll(buf, items);

            return buf;
        }

        private static void InsertAll<T>(GapBuffer<T> buf, IEnumerable<T> items)
        {
            foreach(T item in items)
            {
                buf.Insert(item);
            }
        }
    }
}
