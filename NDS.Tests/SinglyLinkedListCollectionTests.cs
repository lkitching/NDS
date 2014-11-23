using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    /// <summary>Tests for <see cref="SinglyLinkedListCollection{T}"/>.</summary>
    [TestFixture]
    public class SinglyLinkedListCollectionTests
    {
        /// <summary>Tests an empty list can be created.</summary>
        [Test]
        public void ShouldCreateEmpty()
        {
            var list = Create<string>();
            Assert.AreEqual(0, list.Count, "List should be initally empty");
        }

        /// <summary>Tests a list can be initialised from an existing sequence.</summary>
        [Test]
        public void ShouldCreateFromSequence()
        {
            var r = new Random();
            int count = r.Next(1, 200);
            var items = RandomInts(r).Take(count).ToArray();

            var list = new SinglyLinkedListCollection<int>(items);
            CollectionAssert.AreEqual(items, list);
            Assert.AreEqual(count, list.Count, "Unexpected count");
        }

        /// <summary>Tests an item can be added to the front of the list.</summary>
        [Test]
        public void ShouldAddFirst()
        {
            var list = Create<int>();
            int value = new Random().Next();
            list.AddFirst(value);

            Assert.AreEqual(value, list.First, "Failed to update first");
            Assert.AreEqual(1, list.Count, "Failed to increment count");
        }

        /// <summary>Tests the getter for the first list item throws if the list is empty.</summary>
        [Test]
        public void FirstGetterShouldThrowIfListEmpty()
        {
            var list = Create<string>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.First; });
        }

        /// <summary>Tests the first item can be removed.</summary>
        [Test]
        public void ShouldRemoveFirst()
        {
            var list = Create<int>();
            var r = new Random();
            var count = 20;
            int first = -1;

            foreach (int i in RandomInts(r).Take(count))
            {
                list.AddFirst(i);
                first = i;
            }

            var removed = list.RemoveFirst();
            Assert.AreEqual(first, removed, "Unexpected first element in list");
            Assert.AreEqual(count - 1, list.Count, "Failed to adjust count");
        }

        /// <summary>Tests RemoveFirst throws if the list is empty.</summary>
        [Test]
        public void RemoveFirstShouldThrowIfListEmpty()
        {
            var list = Create<int>();
            Assert.Throws<InvalidOperationException>(() => { var _ = list.RemoveFirst(); });
        }

        /// <summary>Test all elements are removed from the list if they all pass the removal predicate.</summary>
        [Test]
        public void ShouldRemoveAll()
        {
            int maxEx = 10;
            var list = Create<int>(Enumerable.Range(0, maxEx).ToArray());

            int removed = list.RemoveWhere(i => i < maxEx);

            Assert.AreEqual(maxEx, removed, "Unexpected number of elements removed");
            Assert.AreEqual(0, list.Count, "Failed to adjust count");
        }

        /// <summary>Tests all items passing the removal predicate are removed from the collection.</summary>
        [Test]
        public void ShouldRemoveWhereMatching()
        {
            var random = new Random();
            var count = 1000;
            var items = RandomInts(random).Take(count).ToArray();

            var list = Create(items);
            Func<int, bool> p = i => i % 2 == 0;

            int removed = list.RemoveWhere(p);
            int expectedRemoved = items.Count(p);

            CollectionAssert.AreEqual(items.Where(i => !p(i)), list, "Unexpected items in the collection");
            Assert.AreEqual(expectedRemoved, removed, "Unexpected number of removed items");
            Assert.AreEqual(count, list.Count + removed, "Failed to adjust count");
        }

        private static SinglyLinkedListCollection<T> Create<T>(params T[] items)
        {
            return new SinglyLinkedListCollection<T>(items);
        }

        private static IEnumerable<int> RandomInts(Random random)
        {
            return Generate(random, r => r.Next());
        }

        private static IEnumerable<T> Generate<T>(Random r, Func<Random, T> gen)
        {
            while (true)
            {
                yield return gen(r);
            }
        }
    }
}
