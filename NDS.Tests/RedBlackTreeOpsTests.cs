using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class RedBlackTreeOpsTests
    {
        [Test]
        public void Should_Insert_Into_Empty_Tree()
        {
            int key = 2;
            string value = "value";
            var newRoot = RedBlackTreeOps.ApplyInsert(null, new ArrayList<SearchBranch<RedBlackNode<int, string>>>(), key, value);

            AssertNode(newRoot, key, value, RBNodeColour.Black);
        }

        [Test]
        public void Should_Insert_Without_Restructuring()
        {
            int key = 7;
            string value = "value";

            var leftChild = CreateNode(4, "left", RBNodeColour.Black);
            var rightChild = CreateNode(13, "right", RBNodeColour.Black);
            var root = new RedBlackNode<int, string>(10, "root", RBNodeColour.Black) { Left = leftChild, Right = rightChild };
            leftChild.Parent = root;
            rightChild.Parent = root;

            var searchPath = new ArrayList<SearchBranch<RedBlackNode<int, string>>>
            {
                new SearchBranch<RedBlackNode<int, string>>(root, BranchDirection.Left),
                new SearchBranch<RedBlackNode<int, string>>(leftChild, BranchDirection.Right)
            };

            var newRoot = RedBlackTreeOps.ApplyInsert(root, searchPath, key, value);

            Assert.AreSame(root, newRoot, "Root should remain unchanged");
            Assert.AreSame(leftChild, newRoot.Left, "Existing tree should be unchanged");
            Assert.AreSame(rightChild, newRoot.Right, "Existing tree should be unchanged");
        }

        [Test]
        public void Insert_Restructure_Left_Left_Red_Uncle()
        {
            var leftChild = CreateNode(5, "left", RBNodeColour.Red);
            var rightChild = CreateNode(13, "right", RBNodeColour.Red);
            var root = new RedBlackNode<int, string>(10, "root", RBNodeColour.Black) { Left = leftChild, Right = rightChild };
            leftChild.Parent = root;
            rightChild.Parent = root;

            //insert into left of left child
            var searchPath = new ArrayList<SearchBranch<RedBlackNode<int, string>>>()
            {
                new SearchBranch<RedBlackNode<int, string>>(root, BranchDirection.Left),
                new SearchBranch<RedBlackNode<int, string>>(leftChild, BranchDirection.Left)
            };

            int key = 3;
            string value = "value";
            var newRoot = RedBlackTreeOps.ApplyInsert(root, searchPath, key, value);

            //left and right children should be recoloured to black
            //root should still be black
            //inserted node should be red
            //structure of tree should otherwise be the same
            Assert.AreSame(root, newRoot, "root");
            Assert.AreEqual(RBNodeColour.Black, root.Colour, "root colour");

            Assert.AreSame(leftChild, root.Left, "left child");
            Assert.AreEqual(RBNodeColour.Black, leftChild.Colour, "left child colour");

            Assert.AreSame(rightChild, root.Right, "right child");
            Assert.AreEqual(RBNodeColour.Black, rightChild.Colour, "right child colour");

            AssertNode(leftChild.Left, key, value, RBNodeColour.Red);
        }

        [Test]
        public void Insert_Restructure_Left_Right_Red_Uncle()
        {
            var leftChild = CreateNode(5, "left", RBNodeColour.Red);
            var rightChild = CreateNode(13, "right", RBNodeColour.Red);
            var root = new RedBlackNode<int, string>(10, "root", RBNodeColour.Black) { Left = leftChild, Right = rightChild };
            leftChild.Parent = root;
            rightChild.Parent = root;

            //insert into right of left child
            var searchPath = new ArrayList<SearchBranch<RedBlackNode<int, string>>>()
            {
                new SearchBranch<RedBlackNode<int, string>>(root, BranchDirection.Left),
                new SearchBranch<RedBlackNode<int, string>>(leftChild, BranchDirection.Right)
            };

            int key = 7;
            string value = "value";
            var newRoot = RedBlackTreeOps.ApplyInsert(root, searchPath, key, value);

            //left and right children should be recolour to black
            //root should still be black
            //inserted node should be red
            //structure of tree should otherwise be the same
            Assert.AreSame(root, newRoot, "root");
            Assert.AreEqual(RBNodeColour.Black, root.Colour, "root colour");

            Assert.AreSame(leftChild, root.Left, "left child");
            Assert.AreEqual(RBNodeColour.Black, leftChild.Colour, "left child colour");

            Assert.AreSame(rightChild, root.Right, "right child");
            Assert.AreEqual(RBNodeColour.Black, rightChild.Colour, "right child colour");

            AssertNode(leftChild.Right, key, value, RBNodeColour.Red);
        }

        [Test]
        public void InsertRestructure_Right_Right_Red_Uncle()
        {
            var leftChild = CreateNode(5, "left", RBNodeColour.Red);
            var rightChild = CreateNode(13, "right", RBNodeColour.Red);
            var root = new RedBlackNode<int, string>(10, "root", RBNodeColour.Black) { Left = leftChild, Right = rightChild };
            leftChild.Parent = root;
            rightChild.Parent = root;

            //insert into right of right child
            var searchPath = new ArrayList<SearchBranch<RedBlackNode<int, string>>>()
            {
                new SearchBranch<RedBlackNode<int, string>>(root, BranchDirection.Right),
                new SearchBranch<RedBlackNode<int, string>>(rightChild, BranchDirection.Right)
            };

            int key = 16;
            string value = "value";
            var newRoot = RedBlackTreeOps.ApplyInsert(root, searchPath, key, value);

            //left and right children should be recolour to black
            //root should still be black
            //inserted node should be red
            //structure of tree should otherwise be the same
            Assert.AreSame(root, newRoot, "root");
            Assert.AreEqual(RBNodeColour.Black, root.Colour, "root colour");

            Assert.AreSame(leftChild, root.Left, "left child");
            Assert.AreEqual(RBNodeColour.Black, leftChild.Colour, "left child colour");

            Assert.AreSame(rightChild, root.Right, "right child");
            Assert.AreEqual(RBNodeColour.Black, rightChild.Colour, "right child colour");

            AssertNode(rightChild.Right, key, value, RBNodeColour.Red);
        }

        [Test]
        public void Insert_Restructure_Right_Left_Red_Uncle()
        {
            var leftChild = CreateNode(5, "left", RBNodeColour.Red);
            var rightChild = CreateNode(13, "right", RBNodeColour.Red);
            var root = new RedBlackNode<int, string>(10, "root", RBNodeColour.Black) { Left = leftChild, Right = rightChild };
            leftChild.Parent = root;
            rightChild.Parent = root;

            //insert into right of right child
            var searchPath = new ArrayList<SearchBranch<RedBlackNode<int, string>>>()
            {
                new SearchBranch<RedBlackNode<int, string>>(root, BranchDirection.Right),
                new SearchBranch<RedBlackNode<int, string>>(rightChild, BranchDirection.Left)
            };

            int key = 11;
            string value = "value";
            var newRoot = RedBlackTreeOps.ApplyInsert(root, searchPath, key, value);

            //left and right children should be recolour to black
            //root should still be black
            //inserted node should be red
            //structure of tree should otherwise be the same
            Assert.AreSame(root, newRoot, "root");
            Assert.AreEqual(RBNodeColour.Black, root.Colour, "root colour");

            Assert.AreSame(leftChild, root.Left, "left child");
            Assert.AreEqual(RBNodeColour.Black, leftChild.Colour, "left child colour");

            Assert.AreSame(rightChild, root.Right, "right child");
            Assert.AreEqual(RBNodeColour.Black, rightChild.Colour, "right child colour");

            AssertNode(rightChild.Left, key, value, RBNodeColour.Red);
        }

        [Test]
        public void Insert_Test()
        {
            var H = CreateNode(26, "H", RBNodeColour.Red);
            var G = CreateNode(11, "G", RBNodeColour.Red);
            var F = CreateNode(8, "F", RBNodeColour.Red);
            var E = new RedBlackNode<int, string>(22, "E", RBNodeColour.Black) { Right = H };
            var D = new RedBlackNode<int, string>(10, "D", RBNodeColour.Black) { Left = F, Right = G };
            var C = new RedBlackNode<int, string>(18, "C", RBNodeColour.Red) { Left = D, Right = E };
            var B = CreateNode(3, "B", RBNodeColour.Black);
            var A = new RedBlackNode<int, string>(7, "A", RBNodeColour.Black) { Left = B, Right = C };

            H.Parent = E;
            G.Parent = D;
            F.Parent = D;
            D.Parent = C;
            E.Parent = C;
            B.Parent = A;
            C.Parent = A;

            int key = 15;
            string value = "value";
            var searchPath = new ArrayList<SearchBranch<RedBlackNode<int, string>>>
            {
                new SearchBranch<RedBlackNode<int, string>>(A, BranchDirection.Right),
                new SearchBranch<RedBlackNode<int, string>>(C, BranchDirection.Left),
                new SearchBranch<RedBlackNode<int, string>>(D, BranchDirection.Right),
                new SearchBranch<RedBlackNode<int, string>>(G, BranchDirection.Right),
            };

            var newRoot = RedBlackTreeOps.ApplyInsert(A, searchPath, key, value);

            Assert.AreSame(D, newRoot, "Unexpected root");
            Assert.AreEqual(RBNodeColour.Black, D.Colour, "Root should be black");

            Assert.AreSame(A, D.Left, "Unexpected left child");
            Assert.AreEqual(RBNodeColour.Red, A.Colour, "Left child should be red");

            Assert.AreSame(C, D.Right, "Unexpected right child");
            Assert.AreEqual(RBNodeColour.Red, C.Colour, "Right child should be red");

            Assert.AreSame(B, A.Left, "Unexpected left left child");
            Assert.AreEqual(RBNodeColour.Black, B.Colour, "Left left child should be black");

            Assert.AreSame(F, A.Right, "Unexpected left right child");
            Assert.AreEqual(RBNodeColour.Black, F.Colour, "Left right child should be black");

            Assert.AreSame(G, C.Left, "Unexpected right left child");
            Assert.AreEqual(RBNodeColour.Black, G.Colour, "Right left child should be black");

            Assert.AreSame(E, C.Right, "Unexpected right right child");
            Assert.AreEqual(RBNodeColour.Black, E.Colour, "Right right child should be black");

            //new node should now be right-left-right child of root
            AssertNode(G.Right, key, value, RBNodeColour.Red);
        }

        [Test]
        public void Should_Delete_Only_Node()
        {
            int key = 43;
            var node = CreateNode(key, "value", RBNodeColour.Black);

            var newRoot = Delete(node, key);
            Assert.IsNull(newRoot, "Tree should be empty");
        }

        [Test]
        public void Should_Replace_With_Successor_If_Red()
        {
            //       (A, 10, Black)
            //      /              \
            // (B, 4, Black)    (C, 12, Red)
            //                              \
            //                           (D, 14, Black)
            var D = CreateNode(14, "D", RBNodeColour.Black);
            var C = new RedBlackNode<int, string>(12, "C", RBNodeColour.Red) { Right = D };
            var B = CreateNode(4, "B", RBNodeColour.Black);
            var A = new RedBlackNode<int, string>(10, "A", RBNodeColour.Black) { Left = B, Right = C };

            var newRoot = Delete(A, 10);

            // expected:
            //      (C, 12, Black)
            //     /              \
            // (B, 4, Black)  (D, 14, Black)
            AssertNode(newRoot, 12, "C", RBNodeColour.Black);
            Assert.AreSame(B, newRoot.Left, "left child should be unchanged");
            Assert.AreSame(D, newRoot.Right, "D should be new right child of root");
        }

        [Test]
        public void Should_Recolour_If_Black_With_Red_Child()
        {
            //         (A, 10, Black)
            //        /              \
            // (B, 4, Black)      (C, 12, Black)
            //                                  \
            //                                (D, 14, Red)
            var D = CreateNode(14, "D", RBNodeColour.Red);
            var C = new RedBlackNode<int, string>(12, "C", RBNodeColour.Black) { Right = D };
            var B = CreateNode(4, "B", RBNodeColour.Black);
            var A = new RedBlackNode<int, string>(10, "A", RBNodeColour.Black) { Left = B, Right = C };

            var newRoot = Delete(A, 10);

            // expected:
            //       (C, 12, Black)
            //       /             \
            // (B, 4, Black)    (D, 14, Black)
            AssertNode(newRoot, 12, "C", RBNodeColour.Black);
            Assert.AreSame(B, newRoot.Left, "Left child should be unchanged");
            Assert.AreSame(D, newRoot.Right, "D should be right child of root");
            Assert.AreEqual(RBNodeColour.Black, newRoot.Right.Colour, "D should be recoloured to black");
        }

        [Test]
        public void Should_Recolour_Black_Sibling_If_Black_Leaf()
        {
            //      (A, 10, Black)
            //     /              \
            // (B, 4, Black)   (C, 12, Black)
            var C = CreateNode(12, "C", RBNodeColour.Black);
            var B = CreateNode(4, "B", RBNodeColour.Black);
            var A = new RedBlackNode<int, string>(10, "A", RBNodeColour.Black) { Left = B, Right = C };
            C.Parent = A;
            B.Parent = A;

            var newRoot = Delete(A, 10);

            //expected:
            //    (C, 12, Black)
            //    /
            // (B, 4, Red)
            AssertNode(newRoot, 12, "C", RBNodeColour.Black);
            Assert.AreSame(B, newRoot.Left, "Left child should be unchanged");
            Assert.AreEqual(RBNodeColour.Red, B.Colour, "Should recolour sibling to red");
        }

        [Test]
        public void Should_Rotate_And_Recolour_Successor_Sibling_If_Red()
        {
            //                (A, 10, Black)
            //               /              \
            //           (B, 6, Red)     (C, 12, Black)
            //          /           \
            //     (D, 4, Black) (E, 8, Black)
            var D = CreateNode(4, "D", RBNodeColour.Black);
            var E = CreateNode(8, "E", RBNodeColour.Black);
            var B = new RedBlackNode<int, string>(6, "B", RBNodeColour.Red) { Left = D, Right = E };
            D.Parent = B;
            E.Parent = B;

            var C = CreateNode(12, "C", RBNodeColour.Black);
            var A = new RedBlackNode<int, string>(10, "A", RBNodeColour.Black) { Left = B, Right = C };
            B.Parent = A;
            C.Parent = A;

            var newRoot = Delete(A, 10);

            //expected:
            //       (B, 6, Black)
            //      /             \
            //   (D, 4, Black)  (C, 12, Black)
            //                 /
            //             (E, 8, Red)
            AssertNode(newRoot, 6, "B", RBNodeColour.Black);
            Assert.AreSame(D, newRoot.Left, "Unexpected left child");
            AssertNode(newRoot.Right, 12, "C", RBNodeColour.Black);
            AssertNode(newRoot.Right.Left, 8, "E", RBNodeColour.Red);
        }

        private static RedBlackNode<TKey, TValue> Delete<TKey, TValue>(RedBlackNode<TKey, TValue> root, TKey key)
        {
            var context = SearchForDelete(root, key);
            return RedBlackTreeOps.ApplyDelete(context.SearchPath, context.MatchPathIndex.Value);
        }

        private static RedBlackNode<TKey, TValue> CreateNode<TKey, TValue>(TKey key, TValue value, RBNodeColour colour)
        {
            return new RedBlackNode<TKey, TValue>(key, value, colour);
        }

        private static ArrayList<SearchBranch<RedBlackNode<TKey, TValue>>> CreatePath<TKey, TValue>(params SearchBranch<RedBlackNode<TKey, TValue>>[] branches)
        {
            return new ArrayList<SearchBranch<RedBlackNode<TKey, TValue>>>(branches);
        }

        private static IBSTDeleteContext<RedBlackNode<TKey, TValue>> SearchForDelete<TKey, TValue>(RedBlackNode<TKey, TValue> root, TKey key)
        {
            return BSTSearch.SearchForDelete<RedBlackNode<TKey, TValue>, TKey, TValue>(root, key, Comparer<TKey>.Default);
        }

        private static void AssertNode<TKey, TValue>(RedBlackNode<TKey, TValue> node, TKey expectedKey, TValue expectedValue, RBNodeColour expectedColour)
        {
            Assert.AreEqual(expectedKey, node.Key);
            Assert.AreEqual(expectedValue, node.Value);
            Assert.AreEqual(expectedColour, node.Colour);
        }
    }
}
