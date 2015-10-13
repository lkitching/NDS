using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class BSTSearchTests
    {
        private BSTNode<int, string> A, B, C, D, E, F;

        /// <summary>Sets up the BST used in these tests.</summary>
        /// <returns>
        ///                   (A, 10)
        ///                  /       \
        ///               (B, 5)   (C, 12)
        ///              /      \
        ///           (D, 2)  (E, 7)
        ///                 \
        ///               (F, 3)
        /// </returns>
        [SetUp]
        public void Setup()
        {
            F = new BSTNode<int, string>(3, "F");
            D = new BSTNode<int, string>(2, "D") { Right = F };
            E = new BSTNode<int, string>(7, "E");
            B = new BSTNode<int, string>(5, "B") { Left = D, Right = E };
            C = new BSTNode<int, string>(12, "C");
            A = new BSTNode<int, string>(10, "A") { Left = B, Right = C };
        }

        [Test]
        [TestCase(4, 4, BSTComparisonResult.This)]
        [TestCase(4, 2, BSTComparisonResult.Left)]
        [TestCase(4, 7, BSTComparisonResult.Right)]
        public void FindKey_Should_Match_Current_Node(int nodeKey, int searchKey, BSTComparisonResult expectedResult)
        {
            var node = new BSTNode<int, string>(nodeKey, "test");
            var result = node.FindKey(searchKey, Comparer<int>.Default);
            Assert.AreEqual(expectedResult, result, "Unexepcted comparison result");
        }

        [Test]
        public void Search_Path_Should_Be_Empty_For_Null_Root()
        {
            var context = BSTSearch.SearchFor<BSTNode<int, string>, int, string>(null, 1, Comparer<int>.Default);
            Assert.AreEqual(0, context.SearchPath.Count);
        }

        [Test]
        public void Search_Should_Fail_For_Null_Root()
        {
            var context = BSTSearch.SearchFor<BSTNode<int, string>, int, string>(null, 1, Comparer<int>.Default);
            Assert.IsFalse(context.Found);
        }

        [Test]
        public void Should_Set_Search_Path_On_Successful_Search()
        {
            var context = Search(A, 7);
            
            //path should contain [(A, Left), (B, Right)]
            CollectionAssert.AreEqual(new[] { CreateBranch(A, BranchDirection.Left), CreateBranch(B, BranchDirection.Right) }, context.SearchPath, "Unexpected search path");
        }

        [Test]
        public void Should_Set_Matching_Node_On_Successful_Search()
        {
            var context = Search(A, 12);
            Assert.AreEqual(C, context.MatchingNode, "Expected to find matching node");
        }

        [Test]
        public void Should_Set_Search_On_Unsuccessful_Search()
        {
            var context = Search(A, 4);
            var expectedSearch = new[] {
                CreateBranch(A, BranchDirection.Left),
                CreateBranch(B, BranchDirection.Left),
                CreateBranch(D, BranchDirection.Right),
                CreateBranch(F, BranchDirection.Right)
            };

            CollectionAssert.AreEqual(expectedSearch, context.SearchPath, "Unexpected search path");
        }

        [Test]
        public void Should_Indicate_Unsuccessful_Search()
        {
            var context = Search(A, 11);
            Assert.IsFalse(context.Found, "Search should fail");
        }

        private SearchBranch<BSTNode<int, string>> CreateBranch(BSTNode<int, string> node, BranchDirection dir)
        {
            return new SearchBranch<BSTNode<int,string>>(node, dir);
        }

        private IBSTSearchContext<BSTNode<int, string>> Search(BSTNode<int, string> root, int key)
        {
            return BSTSearch.SearchFor<BSTNode<int, string>, int, string>(root, key, Comparer<int>.Default);
        }
    }
}
