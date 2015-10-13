using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class BSTNodeTests
    {
        [Test]
        public void Should_Rotate_Left()
        {
            //     A
            //    / \
            //   B   C
            //      / \
            //     D   E
            var D = new TestNode("D");
            var E = new TestNode("E");
            var C = new TestNode("C") { Left = D, Right = E };
            var B = new TestNode("B");
            var A = new TestNode("A") { Left = B, Right = C };

            D.Parent = E.Parent = C;
            B.Parent = C.Parent = A;

            C.RotateLeft();

            // expected:
            //     C
            //    / \
            //   A   E
            //  / \
            // B   D
            Assert.IsNull(C.Parent, "C should be root");
            Assert.AreSame(A, C.Left, "A should be left child of C");
            Assert.AreSame(E, C.Right, "E should be right child of C");

            Assert.AreSame(C, A.Parent, "C should be parent of A");
            Assert.AreSame(B, A.Left, "B should be left child of A");
            Assert.AreSame(D, A.Right, "D should be right child of A");

            Assert.AreSame(C, E.Parent, "C should be parent of E");
            Assert.IsNull(E.Left, "E should have no left child");
            Assert.IsNull(E.Right, "E should have no right child");

            Assert.AreSame(A, B.Parent, "A should be parent of B");
            Assert.IsNull(B.Left, "B should have no left child");
            Assert.IsNull(B.Right, "B should have no right child");

            Assert.AreSame(A, D.Parent, "A should be parent of D");
            Assert.IsNull(D.Left, "D should have no left child");
            Assert.IsNull(D.Right, "D should have no right child");
        }

        [Test]
        public void Should_Rotate_Right()
        {
            //         A
            //        / \
            //       B   C
            //      / \
            //     D   E
            var E = new TestNode("E");
            var D = new TestNode("D");
            var B = new TestNode("B") { Left = D, Right = E };
            var C = new TestNode("C");
            var A = new TestNode("A") { Left = B, Right = C };

            D.Parent = B;
            E.Parent = B;
            C.Parent = A;
            B.Parent = A;

            B.RotateRight();

            // expected:
            //       B
            //      / \
            //     D   A
            //        / \
            //       E   C
            Assert.IsNull(B.Parent, "B should be root");
            Assert.AreSame(D, B.Left, "D should be left child of root");
            Assert.AreSame(A, B.Right, "A should be right child of root");

            Assert.AreSame(B, D.Parent, "B should be parent of D");
            Assert.IsNull(D.Left, "D should have no left child");
            Assert.IsNull(D.Right, "D should have no right child");

            Assert.AreSame(B, A.Parent, "B should be parent of A");
            Assert.AreSame(E, A.Left, "E should be left child of A");
            Assert.AreSame(C, A.Right, "C should be right child of A");

            Assert.AreSame(A, E.Parent, "A should be parent of E");
            Assert.IsNull(E.Left, "E should have no left child");
            Assert.IsNull(E.Right, "E should have no right child");

            Assert.AreSame(A, C.Parent, "A should be parent of C");
            Assert.IsNull(C.Left, "C should have no left child");
            Assert.IsNull(C.Right, "C should have no right child");
        }

        private class TestNode : IBinaryNode<TestNode>, IHasParent<TestNode>
        {
            public TestNode(string name)
            {
                this.Name = name;
            }

            public string Name { get; private set; }
            public TestNode Left { get; set; }
            public TestNode Right { get; set; }
            public TestNode Parent { get; set; }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
