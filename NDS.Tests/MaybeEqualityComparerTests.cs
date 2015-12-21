using System;
using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class MaybeEqualityComparerTests
    {
        [Test]
        public void None_Values_Should_Be_Equal()
        {
            var comp = new MaybeEqualityComparer<int>();
            TestAssert.AreEqual(Maybe.None<int>(), Maybe.None<int>(), comp);
        }

        [Test]
        public void Somes_With_Equal_Values_Should_Be_Equal()
        {
            var comp = new MaybeEqualityComparer<string>(StringComparer.InvariantCultureIgnoreCase);
            TestAssert.AreEqual(Maybe.Some("abcd"), Maybe.Some("ABCD"), comp);
        }

        [Test]
        public void Somes_With_Different_Values_Should_Not_Be_Equal()
        {
            var comp = new MaybeEqualityComparer<int>();
            TestAssert.AreNotEqual(Maybe.Some(1), Maybe.Some(4), comp);
        }

        [Test]
        public void None_Should_Not_Equal_Some()
        {
            var comp = new MaybeEqualityComparer<int>();
            TestAssert.AreNotEqual(Maybe.Some(1), Maybe.None<int>(), comp);
            TestAssert.AreNotEqual(Maybe.None<int>(), Maybe.Some(1), comp);
        }

        [Test]
        public void Nones_Should_Have_Same_Hash_Code()
        {
            var comp = new MaybeEqualityComparer<int>();
            Assert.AreEqual(comp.GetHashCode(Maybe.None<int>()), comp.GetHashCode(Maybe.None<int>()));
        }

        [Test]
        public void Somes_With_Same_Value_Should_Have_Same_Hash_Code()
        {
            var comp = new MaybeEqualityComparer<int>();
            int value = new Random().Next();
            Assert.AreEqual(comp.GetHashCode(Maybe.Some(value)), comp.GetHashCode(Maybe.Some(value)));
        }
    }
}
