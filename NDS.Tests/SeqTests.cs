using System;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="Seq"/>.</summary>
    [TestFixture]
    public class SeqTests
    {
        /// <summary>Tests IterateWhile generates the expected sequence.</summary>
        [Test]
        public void ShouldIterateWhile()
        {
            var r = new Random();
            int count = r.Next(10, 20);

            var seq = Seq.IterateWhile(0, i => i < count, i => i + 1);
            CollectionAssert.AreEqual(Enumerable.Range(0, count), seq, "Unexpected sequence");
        }

        /// <summary>Tests IterateWhile returns an empty sequence if the initial value fails the condition.</summary>
        [Test]
        public void IterateWhileShouldReturnEmptySequenceIfInitialValueFailsCondition()
        {
            var seq = Seq.IterateWhile(10, i => i < 10, i => i * 2);
            CollectionAssert.IsEmpty(seq);
        }
    }
}
