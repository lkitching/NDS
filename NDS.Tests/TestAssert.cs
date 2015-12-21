using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    public static class TestAssert
    {
        public static void IsNone<T>(Maybe<T> mv, string message = "Maybe has value")
        {
            Assert.IsFalse(mv.HasValue, message);
        }

        public static void IsSome<T>(Maybe<T> mv, T expectedValue)
        {
            Assert.IsTrue(mv.HasValue, "Maybe has no value");
            Assert.AreEqual(expectedValue, mv.Value, "Unexpected value in Maybe");
        }

        public static void SetEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, IEqualityComparer<T> comparer = null, string message = "Sets not equal")
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            HashSet<T> expectedSet = new HashSet<T>(expected, comparer);
            HashSet<T> actualSet = new HashSet<T>(actual, comparer);

            Assert.AreEqual(expectedSet.Count, actualSet.Count, message + ": Sets contain different number of elements");

            expectedSet.ExceptWith(actualSet);
            Assert.AreEqual(0, expectedSet.Count, "{0}: Expected elements ({1}) to exist in the set", message, string.Join(", ", expectedSet));
        }

        public static void AreEqual<T>(T expected, T actual, IEqualityComparer<T> comparer)
        {
            bool eq = comparer.Equals(expected, actual);
            Assert.IsTrue(eq, string.Format("Values not equal: Expected {0}, Actual: {1}", expected, actual));
        }

        public static void AreNotEqual<T>(T x, T y, IEqualityComparer<T> comparer)
        {
            bool eq = comparer.Equals(x, y);
            Assert.IsFalse(eq, string.Format("{0} unexpectedly equal to {1}", x, y));
        }
    }
}
