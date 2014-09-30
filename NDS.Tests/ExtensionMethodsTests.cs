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
    }
}
