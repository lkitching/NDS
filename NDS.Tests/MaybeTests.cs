using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NDS 
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void Constructor_Should_Throw_If_Argument_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { var _ = new Maybe<string>(null); }, "Constructor should throw for null argument");
        }

        [Test]
        public void Should_Get_Value()
        {
            int i = 4;
            var m = new Maybe<int>(i);

            Assert.IsTrue(m.HasValue, "Maybe should have value");
            Assert.AreEqual(i, m.Value, "Maybe value should equal argument");
        }

        [Test]
        public void Default_Maybe_Should_Not_Have_Value()
        {
            var m = new Maybe<string>();
            Assert.IsFalse(m.HasValue, "Empty maybe should not have a value");
        }

        [Test]
        public void Get_Should_Throw_For_Empty_Maybe()
        {
            var m = new Maybe<string>();
            Assert.Throws<InvalidOperationException>(() => { var _ = m.Value; }, "Getter should throw for empty maybe");
        }

        [Test]
        public void GetOr_Should_Return_Value()
        {
            int value = 3;
            var m = new Maybe<int>(value);

            Assert.AreEqual(value, m.GetOr(-1), "GetOr should return inner value");
        }

        [Test]
        public void GetOr_Should_Return_Default_For_Empty()
        {
            var m = new Maybe<int>();
            int @default = 1;

            Assert.AreEqual(@default, m.GetOr(@default), "GetOr should return given default for empty");
        }

        [Test]
        public void GetOrFunc_Should_Return_Value()
        {
            string value = "abc";
            var m = new Maybe<string>(value);

            Assert.AreEqual(value, m.GetOr(() => "default"), "GetOr(Func) should return inner value");
        }

        [Test]
        public void GetOrFunc_Should_Return_Default_For_Empty()
        {
            string @default = "default";
            var m = new Maybe<string>();

            Assert.AreEqual(@default, m.GetOr(() => @default), "GetOr(Func) should return value from func for empty");
        }

        [Test]
        public void Should_Map_Value()
        {
            string value = "value";
            var m = new Maybe<string>(value);

            var mapped = m.Select(str => str.ToUpper());
            Assert.AreEqual(value.ToUpper(), mapped.Value, "Select should map inner value");
        }

        [Test]
        public void Should_Map_Empty()
        {
            var m = new Maybe<int>();
            var mapped = m.Select(i => i * 2);
            Assert.IsFalse(mapped.HasValue, "Transform of empty maybe should be empty");
        }

        [Test]
        public void Where_Should_Keep_Value_If_It_Passes_Filter()
        {
            int value = 3;
            var m = new Maybe<int>(value);

            var result = m.Where(i => i < 5);
            Assert.AreEqual(value, result.Value, "Where should keep value if it passes filter");
        }

        [Test]
        public void Where_Should_Filter_Value()
        {
            var m = new Maybe<string>("value");
            var result = m.Where(str => str.Length > 10);

            Assert.IsFalse(result.HasValue, "Where should return empty if value fails filter");
        }

        [Test]
        public void Where_Should_Not_Filter_Empty()
        {
            bool exec = false;
            var m = new Maybe<int>();

            var result = m.Where(_ => { exec = true; return true; });
            Assert.IsFalse(exec, "Where should not run filter for empty maybe");
            Assert.IsFalse(result.HasValue, "Filter of empty should not have value");
        }

        [Test]
        public void SelectMany_Should_Return_Empty()
        {
            bool exec = false;
            var m = new Maybe<int>();
            var result = m.SelectMany(_ => { exec = true; return new Maybe<string>("value"); });

            Assert.IsFalse(exec, "SelectMany should not invoke bind func for empty");
            Assert.IsFalse(result.HasValue, "SelectMany should return empty if source is empty");
        }

        [Test]
        public void SelectMany_Should_Apply_Bind_Func()
        {
            var m = new Maybe<int>(3);
            var result = m.SelectMany(i => new Maybe<string>(new string('a', i)));
            Assert.AreEqual("aaa", result.Value, "SelectMany should apply bind func for value");
        }

        [Test]
        public void Should_Cast_Empty()
        {
            var m = new Maybe<object>();
            var cast = m.Cast<string>();

            Assert.IsFalse(cast.HasValue, "Cast of empty should be empty");
        }

        [Test]
        public void Should_Cast_Value()
        {
            int value = 4;
            var m = new Maybe<object>(value);
            var cast = m.Cast<IComparable<int>>();

            Assert.AreEqual(value, cast.Value, "Cast value should equal original");
        }

        [Test]
        public void OfType_Should_Propagate_Empty()
        {
            var m = new Maybe<int>();
            var conv = m.OfType<object>();

            Assert.IsFalse(conv.HasValue, "OfType of empty should be empty");
        }

        [Test]
        public void OfType_Should_Return_Empty_If_Conversion_Fails()
        {
            var m = new Maybe<string>("value");
            var conv = m.OfType<int>();

            Assert.IsFalse(conv.HasValue, "OfType result should be empty if conversion fails");
        }

        [Test]
        public void OfType_Should_Cast_Value_If_Conversion_Succeeds()
        {
            string value = "value";
            var m = new Maybe<object>(value);
            var conv = m.OfType<IEnumerable<char>>();

            Assert.AreEqual(value, conv.Value);
        }

        [Test]
        public void Sequence_Should_Be_Empty_For_Empty()
        {
            var m = new Maybe<int>();
            CollectionAssert.IsEmpty(m, "Sequence should be empty for empty maybe");
        }

        [Test]
        public void Sequence_Should_Contain_Value()
        {
            int value = 3;
            var m = new Maybe<int>(value);

            CollectionAssert.AreEqual(new[] { value }, m, "Sequence should contain inner value");
        }

        [Test]
        public void Should_Not_Equal_Null()
        {
            var m = new Maybe<int>();
            Assert.IsFalse(m.Equals(null), "Maybe should not equal null");
        }

        [Test]
        public void Should_Not_Equal_Non_Maybe()
        {
            string value = "value";
            var m = new Maybe<string>(value);
            Assert.IsFalse(m.Equals(value), "Maybe should not equal non-maybe value");
        }

        [Test]
        public void Empty_Values_Should_Be_Equal()
        {
            var m1 = new Maybe<int>();
            var m2 = new Maybe<int>();

            string msg = "Empty maybes should be equal";
            Assert.IsTrue(m1.Equals(m2), msg);
            Assert.IsTrue(m2.Equals(m1), msg);
        }

        [Test]
        public void Empty_Should_Not_Equal_Non_Empty()
        {
            var m1 = new Maybe<int>();
            var m2 = new Maybe<int>(4);

            var msg = "Empty maybe should not equal non-empty";
            Assert.IsFalse(m1.Equals(m2), msg);
            Assert.IsFalse(m2.Equals(m1), msg);
        }

        [Test]
        public void Values_Should_Be_Equal_If_Inner_Values_Equal()
        {
            string value = "value";
            var m1 = new Maybe<string>(value);
            var m2 = new Maybe<string>(value);

            string msg = "Non-empty maybes should be equal if their values are equal";
            Assert.IsTrue(m1.Equals(m2), msg);
            Assert.IsTrue(m2.Equals(m1), msg);
        }

        [Test]
        public void Values_Should_Not_Be_Equal_If_Inner_Values_Not_Equal()
        {
            var m1 = new Maybe<int>(1);
            var m2 = new Maybe<int>(2);

            string msg = "Non-empty maybe should not be equal if their value are not equal";
            Assert.IsFalse(m1.Equals(m2), msg);
            Assert.IsFalse(m2.Equals(m1), msg);
        }

        [Test]
        public void Empty_Maybes_Should_Have_Same_HashCodes()
        {
            Assert.AreEqual(new Maybe<int>().GetHashCode(), new Maybe<int>().GetHashCode(), "Empty maybes should have same hash code");
        }

        [Test]
        public void NonEmpty_Maybes_With_Same_Value_Should_Have_Same_HashCodes()
        {
            int value = 7;
            var m1 = new Maybe<int>(value);
            var m2 = new Maybe<int>(value);

            Assert.AreEqual(m1.GetHashCode(), m2.GetHashCode(), "Equal maybes should have same hash code");
        }

        [Test]
        public void None_Should_Override_ToString()
        {
            var m = Maybe.None<string>();
            Assert.AreEqual("None", m.ToString());
        }

        [Test]
        public void Some_Should_Override_ToString()
        {
            string value = "test";
            string expected = string.Format("Some({0})", value);

            var m = Maybe.Some(value);
            Assert.AreEqual(expected, m.ToString());
        }

        //extension methods

        [Test]
        public void Ap_Should_Invoke_Delegate()
        {
            string value = "value";
            var mf = new Maybe<Func<string, int>>(str => str.Length);
            var m = new Maybe<string>(value);

            var result = mf.Ap(m);
            Assert.AreEqual(value.Length, result.Value, "Ap should apply delegate to value if both exist");
        }

        [Test]
        public void Ap_Should_Return_Empty_If_Delegate_Empty()
        {
            int value = 3;
            var mf = new Maybe<Func<int, int>>();
            var m = new Maybe<int>(value);

            var result = mf.Ap(m);
            Assert.IsFalse(result.HasValue, "Ap should return empty if delegate maybe empty");
        }

        [Test]
        public void Ap_Should_Return_Empty_If_Value_Empty()
        {
            var mf = new Maybe<Func<int, int>>(i => i * 2);
            var result = mf.Ap(new Maybe<int>());
            Assert.IsFalse(result.HasValue, "Ap should return empty if value delegate empty");
        }

        [Test]
        public void Join_Should_Return_Empty_If_Outer_Empty()
        {
            var outer = new Maybe<Maybe<string>>();
            var result = outer.Join();

            Assert.IsFalse(result.HasValue, "Join should return empty if outer empty");
        }

        [Test]
        public void Join_Should_Return_Inner()
        {
            var inner = new Maybe<int>(3);
            var outer = new Maybe<Maybe<int>>(inner);

            var result = outer.Join();
            Assert.AreEqual(inner, result);
        }

        [Test]
        public void ToMaybe_Should_Return_Empty_For_Null_Reference_Type()
        {
            string value = null;
            var m = value.ToMaybe();
            Assert.IsFalse(m.HasValue, "ToMaybe should return empty for null reference");
        }

        [Test]
        public void ToMaybe_Should_Wrap_Non_Null_ReferenceType()
        {
            string value = "value";
            var m = value.ToMaybe();
            Assert.AreEqual(value, m.Value, "ToMaybe should return value for non-null reference type");
        }

        [Test]
        public void ToMaybe_Should_Return_Empty_For_Null_Nullable_Struct()
        {
            int? nullable = null;
            var m = nullable.ToMaybe();
            Assert.IsFalse(m.HasValue, "ToMaybe should return empty for null nullable struct");
        }

        [Test]
        public void ToMaybe_Should_Wrap_Non_Null_Nullable_Struct()
        {
            int value = 4;
            int? nullable = value;

            var m = nullable.ToMaybe();
            Assert.AreEqual(value, m.Value, "ToMaybe should wrap nullable struct value");
        }

        [Test]
        public void ToNullable_Should_Return_Null_For_None()
        {
            var m = new Maybe<int>();
            Assert.IsNull(m.ToNullable(), "ToNullable should return null for empty");
        }

        [Test]
        public void ToNullable_Should_Return_Value()
        {
            int value = 4;
            var m = new Maybe<int>(value);
            Assert.AreEqual(value, m.ToNullable(), "ToNullable should return inner value");
        }

        [Test]
        public void ToClass_Should_Return_Null_For_None()
        {
            var m = new Maybe<string>();
            Assert.IsNull(m.ToClass(), "ToClass should return null for empty");
        }

        [Test]
        public void ToClass_Should_Return_Value()
        {
            string value = ":D";
            var m = new Maybe<string>(value);
            Assert.AreEqual(value, m.ToClass(), "ToClass should return inner reference");
        }
    }
}
