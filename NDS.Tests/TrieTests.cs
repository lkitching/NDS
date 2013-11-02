using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class TrieTests
    {
        [Test]
        public void ShouldContainAddedString()
        {
            string s = "Test";
            var trie = new Trie();

            trie.Add(s);
            Assert.IsTrue(trie.Contains(s), "Trie did not contain added string");
        }

        [Test]
        public void ShouldContainPrefix()
        {
            string s = "TestString";
            var trie = new Trie();
            trie.Add(s);

            Assert.IsTrue(trie.Contains("Test"));
        }

        [Test]
        public void ShouldAddStringTwice()
        {
            string s = "Test";
            var trie = new Trie();

            trie.Add(s);
            trie.Add(s);
        }

        [Test]
        public void ShouldRemove()
        {
            string s = "Added";
            var trie = new Trie();

            trie.Add(s);
            bool removed = trie.Remove(s);

            Assert.IsTrue(removed, "Failed to remove string");
            Assert.IsFalse(trie.Contains(s), "Trie still contains removed string");
        }

        [Test]
        public void ShouldRemoveUpToUsedPrefix()
        {
            string prefix = "add";
            string s = prefix + "er";
            string other = prefix + "ing";

            var trie = new Trie();
            trie.Add(s);
            trie.Add(other);

            trie.Remove(s);
            Assert.IsTrue(trie.Contains(prefix));
            Assert.IsFalse(trie.Contains(s));
            Assert.IsFalse(trie.Contains("adde"));
            Assert.IsTrue(trie.Contains(other));
        }
    }
}
