using NUnit.Framework;
using System.Linq;

namespace NDS.Tests
{
    [TestFixture]
    public class LexicographicComparerTests
    {
        [Test]
        public void Empty_Sequences_Should_Be_Equal()
        {
            var comp = new LexicographicComparer<char>();
            Assert.AreEqual(0, comp.Compare("", ""), "Empty sequences should be equal");
        }

        [Test]
        public void Equal_Sequences_Should_Be_Equal()
        {
            var seq = TestGen.NRandomInts(50, 100).ToArray();
            var comp = new LexicographicComparer<int>();

            Assert.AreEqual(0, comp.Compare(seq, seq), "Sequences should be equal");
        }

        [Test]
        public void Should_Be_Smaller()
        {
            var comp = new LexicographicComparer<char>();
            Assert.Less(comp.Compare("aaa", "zzz"), 0);
        }

        [Test]
        public void Should_Be_Larger()
        {
            var comp = new LexicographicComparer<char>();
            Assert.Greater(comp.Compare("aabc", "aaab"), 0);
        }

        [Test]
        public void Prefix_Sequence_Should_Be_Smaller()
        {
            var comp = new LexicographicComparer<int>();
            var pre = TestGen.NRandomInts(50, 100).ToArray();
            var rest = TestGen.NRandomInts(50, 100).ToArray();

            Assert.Less(comp.Compare(pre, pre.Concat(rest)), 0, "Prefix sequence should be smaller");
        }

        [Test]
        public void Suffix_Sequence_Should_Be_Larger()
        {
            var comp = new LexicographicComparer<int>();
            var prefix = TestGen.NRandomInts(50, 100).ToArray();
            var rest = TestGen.NRandomInts(50, 100).ToArray();

            var longer = prefix.Concat(rest);
            Assert.Greater(comp.Compare(longer, prefix), 0, "Longer sequence should be larger");
        }

        [Test]
        public void Should_Be_Symmetric()
        {
            var comp = new LexicographicComparer<int>();
            var x = TestGen.NRandomInts(5, 10).ToArray();
            var y = TestGen.NRandomInts(5, 10).ToArray();

            Assert.AreEqual(comp.Compare(x, y), -1 * comp.Compare(y, x), "Comparison should be symmetric");
        }
    }
}
