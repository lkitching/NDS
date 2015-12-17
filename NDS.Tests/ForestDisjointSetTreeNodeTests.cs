using System;
using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class ForestDisjointSetTreeNodeTests
    {
        [Test]
        public void Constructor_Should_Set_Value()
        {
            string value = "value";
            var node = new ForestDisjointSetTreeNode<string>(value);
            Assert.AreSame(value, node.Value, "Should set value");
        }

        [Test]
        public void Constructor_Should_Set_Parent_To_Self()
        {
            var node = new ForestDisjointSetTreeNode<int>(4);
            Assert.AreSame(node, node.Parent, "Should be own parent initially");
        }

        [Test]
        public void Rank_Should_Be_Zero_Initially()
        {
            var node = new ForestDisjointSetTreeNode<string>("value");
            Assert.AreEqual(0, node.Rank, "Rank should be 0 on construction");
        }

        [Test]
        public void Should_Be_Root_Initially()
        {
            var node = new ForestDisjointSetTreeNode<string>("value");
            Assert.IsTrue(node.IsRoot, "Should be root");
        }

        [Test]
        public void Should_Not_Be_Root_If_Not_Own_Parent()
        {
            var parent = new ForestDisjointSetTreeNode<int>(4);
            var child = new ForestDisjointSetTreeNode<int>(8) { Parent = parent };

            Assert.IsFalse(child.IsRoot, "Should not be root");
        }

        [Test]
        public void Should_Merge_Node_To_Self()
        {
            var node = new ForestDisjointSetTreeNode<int>(2);
            int oldRank = node.Rank;

            ForestDisjointSetTreeNode.MergeTrees(node, node);

            Assert.AreEqual(oldRank, node.Rank, "Rank should be unmodified");
            Assert.AreSame(node, node.Parent, "Node should still be own parent");
        }

        [Test]
        public void Should_Merge_Nodes_With_Different_Ranks()
        {
            var random = new Random();
            int smallerRank = random.Next(0, 10);
            int largerRank = random.Next(smallerRank + 1, 20);

            var smaller = new ForestDisjointSetTreeNode<int>(3) { Rank = smallerRank };
            var larger = new ForestDisjointSetTreeNode<int>(1) { Rank = largerRank };

            var arg1 = TestGen.RandomBool() ? smaller : larger;
            var arg2 = arg1 == smaller ? larger : smaller;

            ForestDisjointSetTreeNode.MergeTrees(arg1, arg2);

            Assert.AreEqual(smallerRank, smaller.Rank, "Smaller rank should be unchanged");
            Assert.AreEqual(largerRank, larger.Rank, "Larger rank should be unchanged");
            Assert.AreSame(larger, smaller.Parent, "Unexpected parent node for smaller rank");
            Assert.IsTrue(larger.IsRoot, "Unexpected root node");
        }

        [Test]
        public void Should_Merge_Nodes_With_Same_Rank()
        {
            int rank = new Random().Next(1, 20);
            var x = new ForestDisjointSetTreeNode<int>(2) { Rank = rank };
            var y = new ForestDisjointSetTreeNode<int>(9) { Rank = rank };

            var arg1 = TestGen.RandomBool() ? x : y;
            var arg2 = arg1 == x ? y : x;

            ForestDisjointSetTreeNode.MergeTrees(arg1, arg2);

            //one tree should have the same rank with the other as parent
            var child = arg1.Rank == rank ? arg1 : arg2;
            var parent = child == arg1 ? arg2 : arg1;

            Assert.AreSame(child.Parent, parent, "Unexpected parent node");
            Assert.AreEqual(rank + 1, parent.Rank, "Unexpected rank for parent");
            Assert.IsTrue(parent.IsRoot, "Unexpected root");
        }
    }
}
