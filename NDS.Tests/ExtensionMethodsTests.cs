using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class ExtensionMethodsTests
    {
        [Test]
        public void NullOrEmtpyShouldReturnEmptyForNullSequence()
        {
            IEnumerable<int> seq = null;
            CollectionAssert.IsEmpty(seq.EmptyIfNull());
        }

        [Test]
        public void NullOrEmptyShouldReturnInputForNonNullSequence()
        {
            var seq = new[] { 1, 2, 3 };
            var orEmpty = seq.EmptyIfNull();

            Assert.AreSame(seq, orEmpty);
        }

        [Test]
        [TestCase(-4, ComparisonResult.Less, Description = "Negative")]
        [TestCase(8, ComparisonResult.Greater, Description = "Positive")]
        [TestCase(0, ComparisonResult.Equal, Description = "Zero")]
        public void To_Comparison_Result_Test(int i, ComparisonResult expectedResult)
        {
            Assert.AreEqual(expectedResult, i.ToComparisonResult());
        }
    }
}
