using System;

using NUnit.Framework;
using NDS.Immutable;

namespace NDS.Tests.Immutable
{
    [TestFixture]
    public class RopeTests
    {
        [Test]
        public void Empty_Should_Have_Length_Zero()
        {
            Assert.AreEqual(0, Rope.Empty.Length, "Unexpected length for empty rope");
        }

        [Test]
        public void Should_Convert_Empty_To_Empty_String()
        {
            Assert.AreEqual(string.Empty, Rope.Empty.ToString(), "Unexpected string representation of empty rope");
        }

        [Test]
        public void Should_Create_From_Null()
        {
            Assert.AreEqual(0, Rope.FromString(null).Length, "Rope from null string should be empty");
        }

        [Test]
        public void Should_Create_From_String()
        {
            var s = "test string";
            var rope = Rope.FromString(s);
            Assert.AreEqual(s, rope.ToString());
        }

        [Test]
        public void Should_Get_At_Index()
        {
            var s = "test string";
            var index = new Random().Next(0, s.Length);
            var r = Rope.FromString(s);

            Assert.AreEqual(s[index], r[index], "Unexpected character at index {0}", index);
        }

        [Test]
        public void Indexer_Should_Throw_If_Index_Negative()
        {
            var r = Rope.FromString("test");
            Assert.Throws<IndexOutOfRangeException>(() => { char _ = r[-1]; });
        }

        [Test]
        public void Indexer_Should_Throw_If_Index_Too_Large()
        {
            var r = Rope.FromString("test");
            Assert.Throws<IndexOutOfRangeException>(() => { char _ = r[r.Length]; });
        }

        [Test]
        public void Should_Append()
        {
            var s1 = "foo";
            var s2 = "bar";
            var appended = Rope.FromString(s1).Append(Rope.FromString(s2));

            string expected = s1 + s2;
            Assert.AreEqual(expected, appended.ToString(), "Unexpected appended value");
        }

        [Test]
        public void Should_Prepend()
        {
            var s1 = "foo";
            var s2 = "bar";
            var prepened = Rope.FromString(s1).Prepend(Rope.FromString(s2));

            string expected = s2 + s1;
            Assert.AreEqual(expected, prepened.ToString(), "Unexpected prepended value");
        }

        [Test]
        public void SplitAt_Zero_Should_Equal_Original()
        {
            var s = "input";
            var split = Rope.FromString(s).SplitAt(0);
            Assert.AreEqual(0, split.Item1.Length, "Left split result should be empty");
            Assert.AreEqual(s, split.Item2.ToString(), "Right split result should equal original");
            
        }

        [Test]
        public void SplitAt_Length_Should_Equal_Original()
        {
            var s = "test string";
            var split = Rope.FromString(s).SplitAt(s.Length);
            Assert.AreEqual(s, split.Item1.ToString(), "Left split result should equal original");
            Assert.AreEqual(0, split.Item2.Length, "Right split result should be empty");
        }

        [Test]
        public void SplitAt_Should_Throw_If_Index_Negative()
        {
            var r = Rope.FromString("test");
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = r.SplitAt(-1); });
        }

        [Test]
        public void Split_At_Should_Throw_If_Index_Greater_Than_Length()
        {
            var r = Rope.FromString("test string");
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = r.SplitAt(r.Length + 1); });
        }

        [Test]
        public void Should_Insert()
        {
            var r = Rope.FromString("foobaz");
            var inserted = r.InsertAt(3, Rope.FromString("bar"));
            Assert.AreEqual("foobarbaz", inserted.ToString(), "Unexpected value after insert");
        }

        [Test]
        public void Should_Remove_Range()
        {
            var r = Rope.FromString("bananarama");
            var removed = r.RemoveRange(4, 8);
            Assert.AreEqual("banama", removed.ToString(), "Unexpected value after removal");
        }
    }
}
