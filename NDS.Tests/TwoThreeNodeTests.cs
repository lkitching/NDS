using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class TwoThreeNodeTests
    {
        private static Random random = new Random();

        [Test]
        public void Should_Create_Two_Node()
        {
            var leftChild = TwoThreeNode.Create2Node(1, "value", null, null);
            var rightChild = TwoThreeNode.Create2Node(2, "value", null, null);
            var parent = TwoThreeNode.Create2Node(3, "parent", leftChild, rightChild);

            Assert.AreEqual(3, parent.KeyAt(TwoThreeKeyIndex.Left), "Unexpected key");
            Assert.AreEqual("parent", parent.ValueAt(TwoThreeKeyIndex.Left), "Unexpected value");
            Assert.AreEqual(leftChild, parent.GetChild(TwoThreeChildIndex.Left), "Unexpected left child");
            Assert.AreEqual(rightChild, parent.GetChild(TwoThreeChildIndex.Right), "Unexpected right child");
        }

        [Test]
        public void Should_Create_Three_Node()
        {
            var leftChild = Create2NodeLeaf();
            var middleChild = Create3NodeLeaf();
            var rightChild = Create2NodeLeaf();

            var k1 = 3;
            var k2 = 7;
            var v1 = "left";
            var v2 = "right";

            var parent = TwoThreeNode.Create3Node(k1, v1, k2, v2, leftChild, middleChild, rightChild);

            Assert.AreEqual(k1, parent.KeyAt(TwoThreeKeyIndex.Left), "Unexpected left key");
            Assert.AreEqual(k2, parent.KeyAt(TwoThreeKeyIndex.Right), "Unexpected right key");
            Assert.AreEqual(v1, parent.ValueAt(TwoThreeKeyIndex.Left), "Unexpected left value");
            Assert.AreEqual(v2, parent.ValueAt(TwoThreeKeyIndex.Right), "Unexpected right value");

            Assert.AreEqual(leftChild, parent.GetChild(TwoThreeChildIndex.Left), "Unexpected left child");
            Assert.AreEqual(middleChild, parent.GetChild(TwoThreeChildIndex.Middle), "Unexpected middle child");
            Assert.AreEqual(rightChild, parent.GetChild(TwoThreeChildIndex.Right), "Unexpected right child");
        }

        [Test]
        public void Should_Set_Value()
        {
            var node = TwoThreeNode.Create3Node(1, "leftValue", 2, "rightValue", null, null, null);
            var newLeftValue = "newLeft";
            var newRightValue = "newRight";

            node.SetValueAt(TwoThreeKeyIndex.Left, newLeftValue);
            Assert.AreEqual(newLeftValue, node.ValueAt(TwoThreeKeyIndex.Left), "Failed to set left value");

            node.SetValueAt(TwoThreeKeyIndex.Right, newRightValue);
            Assert.AreEqual(newRightValue, node.ValueAt(TwoThreeKeyIndex.Right), "Failed to set right value");
        }

        [Test]
        public void Should_Enumerate_2Node_Key_Indexes()
        {
            var node = Create2NodeLeaf();
            CollectionAssert.AreEqual(new[] { TwoThreeKeyIndex.Left }, node.KeyIndexes);
        }

        [Test]
        public void Should_Enumerate_3Node_KeyIndexes()
        {
            var node = Create3NodeLeaf();
            CollectionAssert.AreEqual(new[] { TwoThreeKeyIndex.Left, TwoThreeKeyIndex.Right }, node.KeyIndexes);
        }

        [Test]
        public void Should_Get_2Node_Key_Right_Child_Index()
        {
            var leftChild = Create2NodeLeaf();
            var rightChild = Create2NodeLeaf();

            var parent = TwoThreeNode.Create2Node(1, "value", leftChild, rightChild);
            Assert.AreEqual(TwoThreeChildIndex.Right, parent.KeyRightChildIndex(TwoThreeKeyIndex.Left));
            Assert.Throws<ArgumentException>(() => { var _ = parent.KeyRightChildIndex(TwoThreeKeyIndex.Right); });
        }

        [Test]
        public void Should_Get_3Node_Key_Right_Child_Indexes()
        {
            var leftChild = Create2NodeLeaf();
            var middleChild = Create2NodeLeaf();
            var rightChild = Create2NodeLeaf();

            var parent = TwoThreeNode.Create3Node(1, "leftValue", 2, "rightValue", leftChild, middleChild, rightChild);
            Assert.AreEqual(TwoThreeChildIndex.Middle, parent.KeyRightChildIndex(TwoThreeKeyIndex.Left));
            Assert.AreEqual(TwoThreeChildIndex.Right, parent.KeyRightChildIndex(TwoThreeKeyIndex.Right));
        }

        [Test]
        public void Should_Insert_Left()
        {
            var leftChild = Create2NodeLeaf();
            var rightChild = Create2NodeLeaf();

            var leftKvp = new KeyValuePair<int, string>(2, "value");
            var parent = TwoThreeNode<int, string>.Create2Node(leftKvp, leftChild, rightChild);

            var newKvp = new KeyValuePair<int, string>(1, "new");
            var newLeftChild = Create3NodeLeaf();

            parent.InsertLeft(newKvp, newLeftChild);

            Assert.AreEqual(2, parent.KeyCount, "Unexpected number of keys after insert");

            Assert.AreEqual(newKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Unexpected left pair after insert");
            Assert.AreEqual(leftKvp, parent.PairAt(TwoThreeKeyIndex.Right), "Unexpected right pair after insert");

            AssertChildren(parent, newLeftChild, leftChild, rightChild);
        }

        [Test]
        public void Should_Insert_Right()
        {
            var leftChild = Create2NodeLeaf();
            var rightChild = Create2NodeLeaf();

            var leftKvp = new KeyValuePair<int, string>(1, "value");
            var parent = TwoThreeNode<int, string>.Create2Node(leftKvp, leftChild, rightChild);

            var newKvp = new KeyValuePair<int, string>(3, "new");
            var newRightChild = Create2NodeLeaf();

            parent.InsertRight(newKvp, newRightChild);

            Assert.AreEqual(2, parent.KeyCount, "Unexepcted number of keys after insert");
            Assert.AreEqual(leftKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Unexpected left pair after insert");
            Assert.AreEqual(newKvp, parent.PairAt(TwoThreeKeyIndex.Right), "Unexpected right pair after insert");

            AssertChildren(parent, leftChild, rightChild, newRightChild);
        }

        [Test]
        public void Should_Not_Insert_Into_3Node()
        {
            var parent = Create3NodeParent();
            var newKvp = new KeyValuePair<int, string>(4, "newValue");
            var newChild = Create2NodeLeaf();

            Assert.Throws<InvalidOperationException>(() => { parent.InsertLeft(newKvp, newChild); });
            Assert.Throws<InvalidOperationException>(() => { parent.InsertRight(newKvp, newChild); });
        }

        [Test]
        public void Should_Borrow_Left_Child()
        {
            var leftChild = Create2NodeLeaf();
            var middleChild = Create3NodeLeaf();
            var rightChild = Create3NodeLeaf();
            var leftKvp = new KeyValuePair<int, string>(3, "left");
            var rightKvp = new KeyValuePair<int, string>(6, "right");

            var parent = TwoThreeNode<int, string>.Create3Node(leftKvp, rightKvp, leftChild, middleChild, rightChild);

            var result = parent.BorrowLeftKey();
            Assert.AreEqual(leftKvp, result.BorrowedPair, "Unexpected borowed key");
            Assert.AreEqual(leftChild, result.BorrowedNode, "Unexpected borrowed child");

            Assert.AreEqual(1, parent.KeyCount, "Unexpected number of keys after borrow");
            AssertChildren(parent, middleChild, rightChild);
        }

        [Test]
        public void Should_Borrow_Right_Child()
        {
            var leftChild = Create2NodeLeaf();
            var middleChild = Create2NodeLeaf();
            var rightChild = Create3NodeLeaf();

            var leftKvp = new KeyValuePair<int, string>(5, "left");
            var rightKvp = new KeyValuePair<int, string>(66, "right");
            var parent = TwoThreeNode<int, string>.Create3Node(leftKvp, rightKvp, leftChild, middleChild, rightChild);

            var result = parent.BorrowRightKey();
            Assert.AreEqual(rightKvp, result.BorrowedPair, "Unexpected borrowed key");
            Assert.AreEqual(rightChild, result.BorrowedNode, "Unexpected borrowed child");

            Assert.AreEqual(1, parent.KeyCount);
            AssertChildren(parent, leftChild, middleChild);
        }

        [Test]
        public void Should_Not_Borrow_From_2Node()
        {
            var parent = Create2NodeLeaf();
            Assert.Throws<InvalidOperationException>(() => { parent.BorrowLeftKey(); });
            Assert.Throws<InvalidOperationException>(() => { parent.BorrowRightKey(); });
        }

        [Test]
        public void Left_2Node_Child_Should_Lend_To_Right_Sibling()
        {
            var leftLeftKvp = new KeyValuePair<int, string>(2, "left");
            var leftRightKvp = new KeyValuePair<int, string>(4, "right");
            var leftLeftChild = Create2NodeLeaf();
            var leftMiddleChild = Create2NodeLeaf();
            var leftRightChild = Create2NodeLeaf();
            var leftChild = TwoThreeNode<int, string>.Create3Node(leftLeftKvp, leftRightKvp, leftLeftChild, leftMiddleChild, leftRightChild);

            var rightChild = Create2NodeLeaf();

            var newNode = Create3NodeLeaf();
            var parentKvp = new KeyValuePair<int, string>(6, "parent");
            var parent = TwoThreeNode<int, string>.Create2Node(parentKvp, leftChild, rightChild);

            bool borrowed = parent.TryLendToChildFromSibling(TwoThreeChildIndex.Right, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //left child should be 2-node
            Assert.AreEqual(1, leftChild.KeyCount);
            Assert.AreEqual(leftLeftKvp, leftChild.PairAt(TwoThreeKeyIndex.Left), "Left sibling key");
            AssertChildren(leftChild, leftLeftChild, leftMiddleChild);

            //left child's right key should move to parent
            //parent's key should move to right child
            Assert.AreEqual(leftRightKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Parent key");
            Assert.AreEqual(parentKvp, rightChild.PairAt(TwoThreeKeyIndex.Left), "Right sibling key");

            //left child's right child should move to right sibling
            //new node should be right child
            AssertChildren(rightChild, leftRightChild, newNode);
        }

        [Test]
        public void Right_2Node_Child_Should_Lend_To_Left_Sibling()
        {
            var leftChild = Create2NodeParent();

            var rightLeftKvp = new KeyValuePair<int, string>(6, "left");
            var rightRightKvp = new KeyValuePair<int, string>(8, "right");
            var rightLeftChild = Create2NodeLeaf();
            var rightMiddleChild = Create3NodeLeaf();
            var rightRightChild = Create3NodeLeaf();
            var rightChild = TwoThreeNode<int, string>.Create3Node(rightLeftKvp, rightRightKvp, rightLeftChild, rightMiddleChild, rightRightChild);

            var parentKvp = new KeyValuePair<int, string>(2, "parent");
            var parent = TwoThreeNode<int, string>.Create2Node(parentKvp, leftChild, rightChild);
            var newNode = Create3NodeLeaf();

            bool borrowed = parent.TryLendToChildFromSibling(TwoThreeChildIndex.Left, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //right child should be a 2-node
            Assert.AreEqual(1, rightChild.KeyCount, "Right child should be 2-node after borrow");
            Assert.AreEqual(rightRightKvp, rightChild.PairAt(TwoThreeKeyIndex.Left), "Right sibling key");
            AssertChildren(rightChild, rightMiddleChild, rightRightChild);

            //right child's left key should move to the parent
            //parent's key should move to the left sibling
            Assert.AreEqual(rightLeftKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Parent key");
            Assert.AreEqual(parentKvp, leftChild.PairAt(TwoThreeKeyIndex.Left), "Left sibling key");

            //right child's left child should move to the left sibling
            //new node should be the left child
            AssertChildren(leftChild, newNode, rightLeftChild);
        }

        [Test]
        public void Middle_3Node_Sibling_Should_Lend_To_Left_Child()
        {
            var leftChild = TwoThreeNode.Create2Node(1, "leftValue", Create2NodeLeaf(), Create3NodeLeaf());

            var middleLeftKvp = new KeyValuePair<int, string>(3, "middleLeft");
            var middleRightKvp = new KeyValuePair<int, string>(5, "middleRight");
            var middleLeftChild = Create2NodeLeaf();
            var middleMiddleChild = Create3NodeLeaf();
            var middleRightChild = Create2NodeLeaf();
            var middleChild = TwoThreeNode<int, string>.Create3Node(middleLeftKvp, middleRightKvp, middleLeftChild, middleMiddleChild, middleRightChild);

            var rightChild = Create2NodeParent();

            var parentLeftKvp = new KeyValuePair<int, string>(2, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(8, "parentRight");
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);

            var newNode = Create2NodeLeaf();

            bool borrowed = parent.TryLendToChildFromSibling(TwoThreeChildIndex.Left, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //middle child should now be a 2-node
            //its middle and right children should have moved to the left
            Assert.AreEqual(1, middleChild.KeyCount, "Unexpected number of keys remaining in borrowed-from child");
            AssertChildren(middleChild, middleMiddleChild, middleRightChild);

            //middle-child left kvp should have moved to parent
            //parent's left-key should have moved to left child
            Assert.AreEqual(middleLeftKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Parent left kvp");
            Assert.AreEqual(parentLeftKvp, leftChild.PairAt(TwoThreeKeyIndex.Left), "Left child kvp");

            //new node should be left-child of left child
            //middle sibling's left child should move to left child
            AssertChildren(leftChild, newNode, middleLeftChild);
        }

        [Test]
        public void Middle_3Node_Sibling_Should_Lend_To_Right_Child()
        {
            var leftChild = Create2NodeParent();

            var middleLeftKvp = new KeyValuePair<int, string>(8, "left");
            var middleRightKvp = new KeyValuePair<int, string>(12, "right");
            var middleLeftChild = Create2NodeLeaf();
            var middleMiddleChild = Create2NodeLeaf();
            var middleRightChild = Create3NodeLeaf();
            var middleChild = TwoThreeNode<int, string>.Create3Node(middleLeftKvp, middleRightKvp, middleLeftChild, middleMiddleChild, middleRightChild);

            var rightChild = Create2NodeParent();

            var parentLeftKvp = new KeyValuePair<int, string>(2, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(20, "parentRight");
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);

            var newNode = Create2NodeLeaf();
            var borrowed = parent.TryLendToChildFromSibling(TwoThreeChildIndex.Right, newNode);

            Assert.IsTrue(borrowed, "Failed to borrow");

            //middle child should now be 2-node
            //its right child should have moved to the right sibling
            Assert.AreEqual(1, middleChild.KeyCount, "Unexpected number of keys remaining in borrowed-from child");
            Assert.AreEqual(middleLeftKvp, middleChild.PairAt(TwoThreeKeyIndex.Left));
            AssertChildren(middleChild, middleLeftChild, middleMiddleChild);

            //middle child right kvp should have moved to parent
            //parent's right kvp should be moved to the right child
            Assert.AreEqual(parentRightKvp, rightChild.PairAt(TwoThreeKeyIndex.Left), "Right child key");
            Assert.AreEqual(middleRightKvp, parent.PairAt(TwoThreeKeyIndex.Right), "Parent right key");

            //new node should be right child of right child
            //middle sibling's right child should be left child
            AssertChildren(rightChild, middleRightChild, newNode);
        }

        [Test]
        public void Left_3Node_Sibling_Should_Left_To_Middle_Child()
        {
            var leftLeftKvp = new KeyValuePair<int, string>(1, "left");
            var leftRightKvp = new KeyValuePair<int, string>(2, "right");
            var leftLeftChild = Create2NodeLeaf();
            var leftMiddleChild = Create3NodeLeaf();
            var leftRightChild = Create3NodeLeaf();
            var leftChild = TwoThreeNode<int, string>.Create3Node(leftLeftKvp, leftRightKvp, leftLeftChild, leftMiddleChild, leftRightChild);

            var middleChild = Create2NodeParent();
            var rightChild = Create2NodeParent();

            var parentLeftKvp = new KeyValuePair<int, string>(5, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(10, "parentRight");

            var newNode = Create2NodeLeaf();
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);

            bool borrowed = parent.TryLendToChildFromSibling(TwoThreeChildIndex.Middle, newNode);

            //left child should now be a 2-node
            Assert.AreEqual(1, leftChild.KeyCount, "Unexpected key count in borrowed-from child");
            Assert.AreEqual(leftLeftKvp, leftChild.PairAt(TwoThreeKeyIndex.Left));
            AssertChildren(leftChild, leftLeftChild, leftMiddleChild);

            //left child's left kvp should move to the parent
            //parent's left key should move to the middle child
            Assert.AreEqual(leftRightKvp, parent.PairAt(TwoThreeKeyIndex.Left));
            Assert.AreEqual(parentLeftKvp, middleChild.PairAt(TwoThreeKeyIndex.Left));

            //new node should be right child of middle child
            //left sibling's right child should be be left child
            AssertChildren(middleChild, leftRightChild, newNode);
        }

        [Test]
        public void Right_3Node_Sibling_Should_Lend_To_Middle_Child()
        {
            var leftChild = Create2NodeParent();
            var middleChild = Create2NodeParent();

            var rightLeftKvp = new KeyValuePair<int, string>(10, "left");
            var rightRightKvp = new KeyValuePair<int, string>(20, "right");
            var rightLeftChild = Create3NodeLeaf();
            var rightMiddleChild = Create3NodeLeaf();
            var rightRightChild = Create2NodeLeaf();
            var rightChild = TwoThreeNode<int, string>.Create3Node(rightLeftKvp, rightRightKvp, rightLeftChild, rightMiddleChild, rightRightChild);

            var parentLeftKvp = new KeyValuePair<int, string>(2, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(8, "parentRight");
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);
            var newNode = Create3NodeLeaf();

            var borrowed = parent.TryLendToChildFromSibling(TwoThreeChildIndex.Middle, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //right child should now be a 2-node
            Assert.AreEqual(1, rightChild.KeyCount, "Unexpected key count in borrowed-from parent");
            Assert.AreEqual(rightRightKvp, rightChild.PairAt(TwoThreeKeyIndex.Left));
            AssertChildren(rightChild, rightMiddleChild, rightRightChild);

            //right child's left key should move to the parent
            //parent's right key should move to the middle child
            Assert.AreEqual(parentRightKvp, middleChild.PairAt(TwoThreeKeyIndex.Left));
            Assert.AreEqual(rightLeftKvp, parent.PairAt(TwoThreeKeyIndex.Right));

            //new node should be left child of middle child
            //right sibling's left child should be right child
            AssertChildren(middleChild, newNode, rightLeftChild);
        }

        [Test]
        public void Siblings_Should_Not_Lend_With_No_Spare_Keys()
        {
            var leftChild = Create2NodeParent();
            var middleChild = Create2NodeParent();
            var rightChild = Create2NodeParent();

            var twoNodeParent = TwoThreeNode.Create2Node(5, "value", leftChild, rightChild);
            AssertSiblingsCannotLend(twoNodeParent);

            var threeNodeParent = TwoThreeNode.Create3Node(5, "left", 10, "right", leftChild, middleChild, rightChild);
            AssertSiblingsCannotLend(threeNodeParent);
        }

        private void AssertSiblingsCannotLend(TwoThreeNode<int, string> parent)
        {
            var newNode = Create2NodeLeaf();
            var childIndexes = new List<TwoThreeChildIndex> { TwoThreeChildIndex.Left, TwoThreeChildIndex.Right };
            if (parent.ChildCount == 3)
            {
                childIndexes.Insert(1, TwoThreeChildIndex.Middle);
            }

            foreach(var childIndex in childIndexes)
            {
                Assert.IsFalse(parent.TryLendToChildFromSibling(childIndex, newNode), string.Format("Managed to lend to {0} sibling", childIndex.ToString().ToLower()));
            }
        }

        [Test]
        public void Parent_Should_Lend_To_Left_Child()
        {
            var leftChild = Create2NodeParent();
            var middleChildKvp = new KeyValuePair<int, string>(10, "middle");
            var middleChild = TwoThreeNode<int, string>.Create2Node(middleChildKvp, Create2NodeLeaf(), Create2NodeLeaf());
            var rightChild = Create2NodeParent();

            var newNodeKvp = new KeyValuePair<int, string>(3, "new");
            var newNode = TwoThreeNode<int, string>.Create2Node(newNodeKvp, null, null);

            var parentLeftKvp = new KeyValuePair<int, string>(5, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(15, "parentRight");
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);

            bool borrowed = parent.TryLendToChild(TwoThreeChildIndex.Left, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //parent should now be a 2-node
            Assert.AreEqual(1, parent.KeyCount);
            Assert.AreEqual(parentRightKvp, parent.PairAt(TwoThreeKeyIndex.Left));

            //right child should be unchanged
            Assert.AreEqual(rightChild, parent.GetChild(TwoThreeChildIndex.Right), "Unexpected right child after borrow");

            //left child should be a 3-node merged from the new node and the old middle child
            var newLeftChild = parent.GetChild(TwoThreeChildIndex.Left);

            Assert.AreEqual(2, newLeftChild.KeyCount, "Unexpected key count after borrow");
            Assert.AreEqual(parentLeftKvp, newLeftChild.PairAt(TwoThreeKeyIndex.Left));
            Assert.AreEqual(middleChildKvp, newLeftChild.PairAt(TwoThreeKeyIndex.Right));

            AssertChildren(newLeftChild, newNode, middleChild.GetChild(TwoThreeChildIndex.Left), middleChild.GetChild(TwoThreeChildIndex.Right));
        }

        [Test]
        public void Parent_Should_Lend_To_Middle_Child()
        {
            var leftChildKvp = new KeyValuePair<int, string>(1, "left");
            var leftChild = TwoThreeNode<int, string>.Create2Node(leftChildKvp, Create2NodeLeaf(), Create2NodeLeaf());
            var middleChild = Create2NodeParent();
            var rightChild = Create2NodeParent();

            var newNodeKvp = new KeyValuePair<int, string>(7, "new");
            var newNode = TwoThreeNode<int, string>.Create2Node(newNodeKvp, null, null);

            var parentLeftKvp = new KeyValuePair<int, string>(5, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(15, "parentRight");
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);

            bool borrowed = parent.TryLendToChild(TwoThreeChildIndex.Middle, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //parent should now be a 2-node
            Assert.AreEqual(1, parent.KeyCount);
            Assert.AreEqual(parentRightKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Unexpected pair remaining in parent");

            //right child should be unchanged
            Assert.AreEqual(rightChild, parent.GetChild(TwoThreeChildIndex.Right), "Unexpected right child after borrow");

            //parent's left child should be a 3-node merged from the new node and the old left child
            var newLeftChild = parent.GetChild(TwoThreeChildIndex.Left);

            Assert.AreEqual(2, newLeftChild.KeyCount, "Unexpected key count after borrow");
            Assert.AreEqual(leftChildKvp, newLeftChild.PairAt(TwoThreeKeyIndex.Left), "Left pair in new left child");
            Assert.AreEqual(parentLeftKvp, newLeftChild.PairAt(TwoThreeKeyIndex.Right), "Right pair in new left child");

            AssertChildren(newLeftChild, leftChild.GetChild(TwoThreeChildIndex.Left), leftChild.GetChild(TwoThreeChildIndex.Right), newNode);
        }

        [Test]
        public void Parent_Should_Lend_To_Right_Child()
        {
            var leftChild = Create2NodeParent();
            var middleChildKvp = new KeyValuePair<int, string>(9, "middle");
            var middleChild = TwoThreeNode<int, string>.Create2Node(middleChildKvp, Create2NodeLeaf(), Create3NodeLeaf());
            var rightChild = Create2NodeParent();

            var newNodeKvp = new KeyValuePair<int, string>(20, "new");
            var newNode = TwoThreeNode<int, string>.Create2Node(newNodeKvp, null, null);

            var parentLeftKvp = new KeyValuePair<int, string>(5, "parentLeft");
            var parentRightKvp = new KeyValuePair<int, string>(15, "parentRight");
            var parent = TwoThreeNode<int, string>.Create3Node(parentLeftKvp, parentRightKvp, leftChild, middleChild, rightChild);

            bool borrowed = parent.TryLendToChild(TwoThreeChildIndex.Right, newNode);
            Assert.IsTrue(borrowed, "Failed to borrow");

            //parent should now be a 2-node
            Assert.AreEqual(1, parent.KeyCount);
            Assert.AreEqual(parentLeftKvp, parent.PairAt(TwoThreeKeyIndex.Left), "Unexpected pair remaining in parent");

            //left child should be unchanged
            Assert.AreEqual(leftChild, parent.GetChild(TwoThreeChildIndex.Left), "Unexpected left child after borrow");

            //parent's right child should be a 3-node merged from the new node and the old middle
            var newRightChild = parent.GetChild(TwoThreeChildIndex.Right);

            Assert.AreEqual(2, newRightChild.KeyCount, "Unexpected key count after borrow");
            Assert.AreEqual(middleChildKvp, newRightChild.PairAt(TwoThreeKeyIndex.Left), "Left pair in new left child");
            Assert.AreEqual(parentRightKvp, newRightChild.PairAt(TwoThreeKeyIndex.Right), "Right pair in new left child");

            AssertChildren(newRightChild, middleChild.GetChild(TwoThreeChildIndex.Left), middleChild.GetChild(TwoThreeChildIndex.Right), newNode);
        }

        [Test]
        public void Parent_2Node_Should_Not_Lend_To_Children()
        {
            var leftChild = Create2NodeLeaf();
            var rightChild = Create2NodeLeaf();
            var parent = TwoThreeNode.Create2Node(2, "parent", leftChild, rightChild);

            Assert.IsFalse(parent.TryLendToChild(TwoThreeChildIndex.Left, null), "Left child");
            Assert.IsFalse(parent.TryLendToChild(TwoThreeChildIndex.Right, null), "Right child");
        }

        [Test]
        public void Should_Merge_From_Left_Child()
        {
            var leftChild = Create2NodeLeaf();

            var rightChildKvp = new KeyValuePair<int, string>(5, "right");
            var rightChild = TwoThreeNode<int, string>.Create2Node(rightChildKvp, null, null);

            var parentKvp = new KeyValuePair<int, string>(2, "parent");
            var parent = TwoThreeNode<int, string>.Create2Node(parentKvp, leftChild, rightChild);

            var merged = parent.MergeChild(TwoThreeChildIndex.Left);
            Assert.AreEqual(merged.PairAt(TwoThreeKeyIndex.Left), parentKvp, "Unexpected left key of merged node");
            Assert.AreEqual(merged.PairAt(TwoThreeKeyIndex.Right), rightChildKvp, "Right key of merged node");
        }

        [Test]
        public void Should_Merge_From_Right_Child()
        {
            var leftChildKvp = new KeyValuePair<int, string>(1, "left");
            var leftChild = TwoThreeNode<int, string>.Create2Node(leftChildKvp, null, null);

            var rightChild = Create2NodeLeaf();

            var parentKvp = new KeyValuePair<int, string>(4, "parent");
            var parent = TwoThreeNode<int, string>.Create2Node(parentKvp, leftChild, rightChild);

            var merged = parent.MergeChild(TwoThreeChildIndex.Right);
            Assert.AreEqual(merged.PairAt(TwoThreeKeyIndex.Left), leftChildKvp, "Left key of merged node");
            Assert.AreEqual(merged.PairAt(TwoThreeKeyIndex.Right), parentKvp, "Right key of merged node");
        }

        [Test]
        public void Should_Not_Merge_3Node_With_Children()
        {
            var parent = Create3NodeParent();
            Assert.Throws<InvalidOperationException>(() => { parent.MergeChild(TwoThreeChildIndex.Left); });
            Assert.Throws<InvalidOperationException>(() => { parent.MergeChild(TwoThreeChildIndex.Right); });
        }

        [Test]
        public void Should_Iterate_Key_Pairs()
        {
            var leftChild = TwoThreeNode.Create2Node(2, "value2", null, null);
            var middleChild = TwoThreeNode.Create3Node(5, "value5", 8, "value8", null, null, null);
            var rightChild = TwoThreeNode.Create3Node(13, "value13", 15, "value15", null, null, null);

            var parent = TwoThreeNode.Create3Node(3, "value3", 10, "value10", leftChild, middleChild, rightChild);

            var expectedPairs = new[] { 2, 3, 5, 8, 10, 13, 15 }.Select(i => new KeyValuePair<int, string>(i, "value" + i));
            var actualPairs = TwoThreeNode.IterateKeyPairsFrom(parent).ToArray();

            CollectionAssert.AreEqual(expectedPairs, TwoThreeNode.IterateKeyPairsFrom(parent));
        }

        private static void AssertChildren<TKey, TValue>(TwoThreeNode<TKey, TValue> parent, params TwoThreeNode<TKey, TValue>[] expectedChildren)
        {
            Assert.AreEqual(parent.ChildCount, expectedChildren.Length, "Unexpected number of children");

            if (parent.IsLeaf) return;

            AssertChild(parent, TwoThreeChildIndex.Left, expectedChildren[0]);
            if (parent.ChildCount == 3)
            {
                AssertChild(parent, TwoThreeChildIndex.Middle, expectedChildren[1]);
            }
            AssertChild(parent, TwoThreeChildIndex.Right, expectedChildren[parent.ChildCount - 1]);
        }

        private static void AssertChild<TKey, TValue>(TwoThreeNode<TKey, TValue> parent, TwoThreeChildIndex childIndex, TwoThreeNode<TKey, TValue> expectedChild)
        {
            Assert.AreEqual(expectedChild, parent.GetChild(childIndex), string.Format("Unexpected {0} child", childIndex.ToString().ToLower()));
        }

        private static TwoThreeNode<int, string> Create2NodeLeaf()
        {
            int key = random.Next();
            return TwoThreeNode.Create2Node(key, "value" + key, null, null);
        }

        private static TwoThreeNode<int, string> Create3NodeLeaf()
        {
            int leftKey = random.Next(int.MaxValue);
            int rightKey = leftKey + 1;
            return TwoThreeNode.Create3Node(leftKey, "value" + leftKey, rightKey, "value" + rightKey, null, null, null);
        }

        private static TwoThreeNode<int, string> Create2NodeParent()
        {
            return TwoThreeNode.Create2Node(2, "parentValue", Create2NodeLeaf(), Create2NodeLeaf());
        }

        private static TwoThreeNode<int, string> Create3NodeParent()
        {
            var leftChild = Create2NodeLeaf();
            var middleChild = Create3NodeLeaf();
            var rightChild = Create3NodeLeaf();

            return TwoThreeNode.Create3Node(1, "leftValue", 40000, "rightValue", leftChild, middleChild, rightChild);
        }
    }
}
