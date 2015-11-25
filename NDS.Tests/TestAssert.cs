﻿using NUnit.Framework;
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
    }
}
