using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    public abstract class MapTests
    {
        [Test]
        public void Initial_Count_Should_Be_Zero()
        {
            var m = Create();
            Assert.AreEqual(0, m.Count, "Initial count should be 0");
        }

        [Test]
        public void TryAdd_Should_Increment_Count()
        {
            var m = Create();
            m.TryAdd(2, "value");

            Assert.AreEqual(1, m.Count, "Failed to adjust count after add");
        }

        [Test]
        public void TryAdd_Should_Return_False_If_Key_Exists()
        {
            var m = Create();
            bool addedFirst = m.TryAdd(1, "value1");
            Assert.IsTrue(addedFirst, "Failed to add mapping");

            bool addedSecond = m.TryAdd(1, "value2");
            Assert.IsFalse(addedSecond, "Added value for existing key");
        }

        [Test]
        public void Should_Retrieve_After_Add()
        {
            var map = Create();
            int key = new Random().Next();
            string value = "test";

            bool added = map.TryAdd(key, value);
            Assert.IsTrue(added, "Failed to add mapping");

            var mv = map.Get(key);
            TestAssert.IsSome(mv, value);
        }

        [Test]
        public void Assoc_Should_Increment_Count()
        {
            var map = Create();
            map.Assoc(2, "value");

            Assert.AreEqual(1, map.Count, "Failed to increment count after Assoc");
        }

        [Test]
        public void Should_Retrieve_After_Assoc()
        {
            var r = new Random();
            var map = Create();
            int count = r.Next(2000, 3000);
            var keys = TestGen.RandomInts(r).Take(count).ToArray();

            foreach (int key in keys)
            {
                string value = "value" + key;
                map.Assoc(key, value);
                var result = map.Get(key);

                TestAssert.IsSome(map.Get(key), value);
            }
        }

        [Test]
        public void Assoc_Should_Replace_Exising_Value()
        {
            var map = Create();
            int key = new Random().Next();
            var value1 = "initial";
            var value2 = "modified";

            map.Assoc(key, value1);
            map.Assoc(key, value2);

            TestAssert.IsSome(map.Get(key), value2);
        }

        [Test]
        public void Assoc_Should_Not_Modify_Count_When_Replacing()
        {
            var map = Create();
            int key = new Random().Next();
            map.Assoc(key, "value1");
            map.Assoc(key, "value2");

            Assert.AreEqual(1, map.Count, "Unexpected count after replace");
        }

        [Test]
        public void Retrieve_Should_Fail_After_Delete()
        {
            var map = Create();
            int key = new Random().Next();

            map.Add(key, "value");
            bool deleted = map.Delete(key);

            Assert.IsTrue(deleted, "Failed to delete item");
            TestAssert.IsNone(map.Get(key));
        }

        [Test]
        public void Delete_Should_Return_False_If_Key_Does_Not_Exist()
        {
            var map = Create();
            bool deleted = map.Delete(new Random().Next());

            Assert.IsFalse(deleted, "Deleted non-existent item");
        }

        [Test]
        public void Delete_Should_Decrement_Count_If_Key_Exists()
        {
            var r = new Random();
            int count = r.Next(20, 100);
            var map = CreateAndPopulate(count);

            Assert.AreEqual(count, map.Count, "Unexpected count before delete");
            bool deleted = map.Delete(count);
            Assert.IsTrue(deleted, "Failed to delete item");
            Assert.AreEqual(count - 1, map.Count, "Failed to decrement count after delete");
        }

        [Test]
        public void Delete_Should_Not_Modify_Count_If_Key_Does_Not_Exist()
        {
            int count = new Random().Next(20, 100);
            var map = CreateAndPopulate(count);
 
            Assert.AreEqual(count, map.Count, "Unexpected count before delete");
            bool deleted = map.Delete(count + 1);
            Assert.IsFalse(deleted, "Deleted non-existent item");
            Assert.AreEqual(count, map.Count, "Decremented count after delete failed");
        }

        [Test]
        public void Clear_Should_Reset_Count()
        {
            var map = CreateAndPopulate(new Random().Next(20, 100));
            map.Clear();
            Assert.AreEqual(0, map.Count, "Failed to reset count after clear");
        }

        [Test]
        public void Retrieve_Should_Fail_After_Clear()
        {
            var r = new Random();
            int count = r.Next(100, 200);
            var keys = TestGen.RandomInts(r).Take(count).ToArray();

            var map = Create();
            PopulateWith(map, keys);

            map.Clear();

            foreach (int i in keys)
            {
                Assert.IsFalse(map.Get(i).HasValue, "Found value for key after clear");
            }
        }

        [Test]
        public void Enumeration_Should_Be_Empty_For_Empty_Map()
        {
            var map = Create();
            CollectionAssert.IsEmpty(map);
        }

        [Test]
        public void Should_Enumerate_Key_Value_Pairs()
        {
            var keys = TestGen.NRandomInts(200, 300).Distinct();
            var pairs = keys.Select(k => new KeyValuePair<int, string>(k, "value" + k)).ToArray();
            var map = Create();

            foreach (var p in pairs)
            {
                map.Add(p);
            }

            CollectionAssert.AreEquivalent(pairs, map, "Unexpected pairs in map");
        }

        private IMap<int, string> CreateAndPopulate(int count)
        {
            var map = Create();
            PopulateWith(map, Enumerable.Range(1, count));

            return map;
        }

        protected static void PopulateWith(IMap<int, string> map, IEnumerable<int> keys)
        {
            foreach (int i in keys)
            {
                map.Assoc(i, "value" + i);
            }
        }

        private IMap<int, string> Create()
        {
            return CreateMap<int, string>();
        }

        protected abstract IMap<TKey, TValue> CreateMap<TKey, TValue>();
    }
}
