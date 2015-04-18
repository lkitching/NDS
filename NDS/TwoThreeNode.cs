using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    /// <summary>Represents a node in a two-three tree.</summary>
    /// <typeparam name="TKey">Key type for this node.</typeparam>
    /// <typeparam name="TValue">Value type for this node.</typeparam>
    public class TwoThreeNode<TKey, TValue>
    {
        private TwoThreeNode()
        {
            this.KeyValues = new KeyValuePair<TKey, TValue>[2];
            this.Children = new TwoThreeNode<TKey, TValue>[3];
        }

        /// <summary>Gets the number of keys in this node.</summary>
        public int KeyCount { get; set; }

        private KeyValuePair<TKey, TValue>[] KeyValues { get; set; }
        private TwoThreeNode<TKey, TValue>[] Children { get; set; }

        /// <summary>Gets the valid key indexes for this node in left-to-right order.</summary>
        public IEnumerable<TwoThreeKeyIndex> KeyIndexes
        {
            get
            {
                yield return TwoThreeKeyIndex.Left;
                if (this.KeyCount == 2)
                {
                    yield return TwoThreeKeyIndex.Right;
                }
            }
        }
        public TwoThreeChildIndex? NextChildIndex(TwoThreeChildIndex index)
        {
            if (this.IsLeaf) return null;
            switch (index)
            {
                case TwoThreeChildIndex.Left:
                    if (this.ChildCount == 2) return TwoThreeChildIndex.Right;
                    else
                    {
                        Debug.Assert(this.ChildCount == 3);
                        return TwoThreeChildIndex.Middle;
                    }
                case TwoThreeChildIndex.Middle: return TwoThreeChildIndex.Right;
                case TwoThreeChildIndex.Right: return null;
                default: throw new ArgumentOutOfRangeException("Invalid child index");
            }
        }

        /// <summary>
        /// Gets the right index of the child for the given key index.
        /// The left key of the root node of the returned child index should be the smallest key larger than the key in this node.
        /// </summary>
        /// <param name="keyIndex">The key index.</param>
        /// <returns>The child to the right of the key in this node.</returns>
        /// <exception cref="ArgumentException">If <paramref name="keyIndex"/> is not valid for this node.</exception>
        public TwoThreeChildIndex KeyRightChildIndex(TwoThreeKeyIndex keyIndex)
        {
            switch (keyIndex)
            {
                case TwoThreeKeyIndex.Left:
                    return this.KeyCount == 2 ? TwoThreeChildIndex.Middle : TwoThreeChildIndex.Right;
                case TwoThreeKeyIndex.Right:
                    if(this.KeyCount == 2) return TwoThreeChildIndex.Right;
                    else throw new ArgumentException("No right key in 2-node");
                default: throw new ArgumentOutOfRangeException("Invalid key index");
            }
        }

        /// <summary>Whether this node is a leaf (i.e. has no children).</summary>
        public bool IsLeaf
        {
            get
            {
                bool isLeaf = Array.TrueForAll(this.Children, n => n == null);
                return isLeaf;
            }
        }

        /// <summary>Gets the number of children belonging to this node.</summary>
        public int ChildCount
        {
            get { return this.IsLeaf ? 0 : this.KeyCount + 1; }
        }

        /// <summary>Indicates whether this node can lend a key to a child or sibling having its only key deleted.</summary>
        public bool CanBorrowKey
        {
            get { return this.KeyCount == 2; }
        }

        public void SwapPairWith(TwoThreeKeyIndex thisKeyIndex, TwoThreeNode<TKey, TValue> other, TwoThreeKeyIndex otherKeyIndex)
        {
            int thisIdx = KeyIndexToInt(thisKeyIndex);
            int otherIdx = KeyIndexToInt(otherKeyIndex);

            var tmp = this.KeyValues[thisIdx];
            this.KeyValues[thisIdx] = other.KeyValues[otherIdx];
            other.KeyValues[otherIdx] = tmp;
        }

        public void FixupAt(TwoThreeChildIndex childIndex, TwoThreeNodeSplit<TKey, TValue> split)
        {
            if (!(childIndex == TwoThreeChildIndex.Left || childIndex == TwoThreeChildIndex.Right))
            {
                throw new ArgumentOutOfRangeException("childIndex", "Can only fixup a node split into a 2-node");
            }

            var nodeKvp = this.KeyValues[0];

            //move keys and child nodes to the right of insert position over
            if (childIndex == TwoThreeChildIndex.Left)
            {
                //current key is larger than new key so move existing to the right
                this.KeyValues[1] = nodeKvp;
                this.KeyValues[0] = split.MiddleKeyPair;

                //move existing right child over and set children from split
                this.Children[2] = this.Children[1];
                this.Children[0] = split.Left;
                this.Children[1] = split.Right;
            }
            else
            {
                //new key is larger than existing key so make it the right key
                this.KeyValues[1] = split.MiddleKeyPair;

                //set children from split result
                this.Children[1] = split.Left;
                this.Children[2] = split.Right;
            }

            this.KeyCount = 2;
        }

        public void InsertLeft(KeyValuePair<TKey, TValue> kvp, TwoThreeNode<TKey, TValue> leftChild)
        {
            this.GuardCanInsert();

            //move key over and insert new left key
            this.KeyValues[1] = this.KeyValues[0];
            this.KeyValues[0] = kvp;

            //move children over and insert new left child
            this.Children[2] = this.Children[1];
            this.Children[1] = this.Children[0];
            this.Children[0] = leftChild;

            this.KeyCount = 2;
        }

        public void InsertRight(KeyValuePair<TKey, TValue> kvp, TwoThreeNode<TKey, TValue> rightChild)
        {
            this.GuardCanInsert();

            this.KeyValues[1] = kvp;
            this.Children[2] = rightChild;

            this.KeyCount = 2;
        }

        /// <summary>Removes the left key and child from this node, turning it into a 2-node.</summary>
        /// <returns>A result contining the key and child removed from this node.</returns>
        /// <exception cref="InvalidOperationException">If this node is a 2-node.</exception>
        public TwoThreeNodeBorrowResult<TKey, TValue> BorrowLeftKey()
        {
            GuardCanBorrow();

            var removedPair = this.KeyValues[0];
            var removedChild = this.Children[0];

            //move key and children over
            this.KeyValues[0] = this.KeyValues[1];
            this.Children[0] = this.Children[1];
            this.Children[1] = this.Children[2];

            this.KeyValues[1] = default(KeyValuePair<TKey, TValue>);
            this.Children[2] = null;

            this.KeyCount = 1;

            return new TwoThreeNodeBorrowResult<TKey,TValue>(removedPair, removedChild);
        }

        /// <summary>Removes the right key and child from this node, turning it into a 2-node.</summary>
        /// <returns>A result containing the key and child removed from this node.</returns>
        /// <exception cref="InvalidOperationException">if this node is a 2-node.</exception>
        public TwoThreeNodeBorrowResult<TKey, TValue> BorrowRightKey()
        {
            GuardCanBorrow();

            var removedKey = this.KeyValues[1];
            var removedChild = this.Children[2];

            this.KeyValues[1] = default(KeyValuePair<TKey, TValue>);
            this.Children[2] = null;
            this.KeyCount = 1;

            return new TwoThreeNodeBorrowResult<TKey,TValue>(removedKey, removedChild);
        }

        private void ShrinkTo2Node(KeyValuePair<TKey, TValue> kvp, TwoThreeNode<TKey, TValue> leftChild, TwoThreeNode<TKey, TValue> rightChild)
        {
            Debug.Assert(this.KeyCount == 2, "Cannot shrink node");

            this.KeyValues[0] = kvp;
            this.Children[0] = leftChild;
            this.Children[1] = rightChild;
            this.KeyCount = 1;
        }

        /// <summary>
        /// Sees if any sibling of the child of this node at the given index has a spare key which can be lent to the child.
        /// If any spare key exists, the keys are re-arranged between the children and this parent node so the spare key is transfered
        /// to the child in need. The order of keys between this node and its children are maintained.
        /// </summary>
        /// <param name="childIndex">Index of the child needing to borrow from a sibling.</param>
        /// <param name="attached">The node being propagated up the tree.</param>
        /// <returns>True if a sibling of the child at <paramref name="childIndex"/> could lend a key.</returns>
        public bool TryLendToChildFromSibling(TwoThreeChildIndex childIndex, TwoThreeNode<TKey, TValue> attached)
        {
            var childNode = this.GetChild(childIndex);

            if (childIndex == TwoThreeChildIndex.Left)
            {
                //removing from left child
                //try borrow from nearest sibling (parent's middle child) first
                var middleSibling = this.Children[1];
                if (middleSibling.CanBorrowKey)
                {
                    var removed = middleSibling.BorrowLeftKey();

                    //borrow left key from sibling
                    //move siblings left key into the parent and move the parent's left key into this node
                    childNode.KeyValues[0] = this.KeyValues[0];
                    this.KeyValues[0] = removed.BorrowedPair;

                    //move left child from middle sibling to right-child of deleted-from node
                    childNode.Children[0] = attached;
                    childNode.Children[1] = removed.BorrowedNode;

                    return true;
                }

                //try to borrow from parent's right-child if it exists
                if (this.KeyCount == 2)
                {
                    var rightSibling = this.Children[2];
                    if (rightSibling.CanBorrowKey)
                    {
                        var borrowResult = rightSibling.BorrowLeftKey();

                        //right sibling has a spare key
                        //move it to the middle sibling so we are in the same position as above and fix in recusive call
                        middleSibling.InsertRight(this.KeyValues[1], borrowResult.BorrowedNode);
                        this.KeyValues[1] = borrowResult.BorrowedPair;

                        bool borrowed = this.TryLendToChildFromSibling(childIndex, attached);
                        Debug.Assert(borrowed, "Failed to borrow from sibling with two keys");
                        return borrowed;
                    }
                    else return false;
                }
                else
                {
                    //parent only has two children so no other sibling to borrow from
                    return false;
                }
            }
            else if (childIndex == TwoThreeChildIndex.Middle)
            {
                //try to borrow right key from left child
                var leftSibling = this.Children[0];
                if (leftSibling.CanBorrowKey)
                {
                    var borrowResult = leftSibling.BorrowRightKey();

                    //swap left key of parent with right key of left sibling node
                    childNode.KeyValues[0] = this.KeyValues[0];
                    this.KeyValues[0] = borrowResult.BorrowedPair;

                    //move right child from left sibling to left child of deleted-from node
                    childNode.Children[0] = borrowResult.BorrowedNode;
                    childNode.Children[1] = attached;
                }

                //try to borrow left key from right child if it exists
                if (this.KeyCount == 2)
                {
                    var rightSibling = this.Children[2];
                    if (rightSibling.CanBorrowKey)
                    {
                        var borrowResult = rightSibling.BorrowLeftKey();

                        //borrow left key from right sibling
                        //move right-key from parent into the current node and move left-key from right-child into parent
                        int parentRightKeyIndex = this.KeyCount - 1;
                        childNode.KeyValues[0] = this.KeyValues[parentRightKeyIndex];
                        this.KeyValues[parentRightKeyIndex] = borrowResult.BorrowedPair;

                        //move left child from right sibling to right child of current node
                        childNode.Children[0] = attached;
                        childNode.Children[1] = borrowResult.BorrowedNode;

                        return true;
                    }
                    else return false;
                }
                else return false;
            }
            else
            {
                //see if child can borrow from left-most sibling
                var nearestLeftSibling = this.Children[this.ChildCount - 2];
                if (nearestLeftSibling.CanBorrowKey)
                {
                    var borrowResult = nearestLeftSibling.BorrowRightKey();

                    //borrow right key from left sibling (middle child of parent)
                    //move right key from parent into current node and move right key from left sibling into the parent
                    int parentKeyIndex = this.KeyCount - 1;
                    childNode.KeyValues[0] = this.KeyValues[parentKeyIndex];
                    this.KeyValues[parentKeyIndex] = borrowResult.BorrowedPair;

                    //move right-child of left sibling to deleted-from node
                    childNode.Children[0] = borrowResult.BorrowedNode;
                    childNode.Children[1] = attached;

                    return true;
                }

                if (this.ChildCount == 3)
                {
                    //see if left sibling has a spare key to borrow
                    var leftSibling = this.Children[0];
                    if (leftSibling.CanBorrowKey)
                    {
                        //left sibling has a spare key so move it's left child to the middle sibling
                        //then the middle sibling has a spare key so we are in the same situation as above
                        //move right key of left sibling to the left key of the parent and the left key of the parent
                        //to the left key of the middle sibling
                        var borrowResult = leftSibling.BorrowRightKey();

                        nearestLeftSibling.InsertLeft(this.KeyValues[0], borrowResult.BorrowedNode);
                        this.KeyValues[0] = borrowResult.BorrowedPair;

                        bool borrowed = this.TryLendToChildFromSibling(childIndex, attached);
                        Debug.Assert(borrowed, "Failed to borrow from sibling with spare key");
                        return borrowed;
                    }
                }

                //no siblings have spare keys
                return false;
            }
        }

        /// <summary>
        /// Lends a spare key and child in this node if one exists to the child at the given index.
        /// If one is available, the key-value pair and child are removed from this node, turning it into a 2-node.
        /// </summary>
        /// <param name="childIndex">The index of the child to try lend to.</param>
        /// <param name="attached">The node being propagated up the tree.</param>
        /// <returns>True if this node had a node to lend to the child.</returns>
        public bool TryLendToChild(TwoThreeChildIndex childIndex, TwoThreeNode<TKey, TValue> attached)
        {
            int childIdx = ChildIndexToInt(childIndex);
            if (this.IsLeaf) throw new InvalidOperationException("No children to lend to");

            for (int i = 0; i <= this.KeyCount; ++i)
            {
                if (this.Children[i].KeyCount == 2)
                {
                    string message = i == childIdx
                        ? "Child has two keys and does not need to borrow"
                        : string.Format("Could have borrowed from child {0}");

                    throw new InvalidOperationException(message);
                }
            }

            //can only borrow if this node has two keys
            if (this.KeyCount == 1) return false;

            //parent has two keys so the node to delete from has two siblings, each with one key
            //(since borrowing from the siblings failed).
            //the node being deleted from will borrow TWO keys - one from the parent and one from the
            //nearest sibling. The parent will then have a single key and two children.
            if (childIndex == TwoThreeChildIndex.Left)
            {
                //removing left child
                //borrow left key from parent and key from right sibling (middle child of the parent)
                var sibling = this.Children[1];
                var newNode = TwoThreeNode<TKey, TValue>.Create3Node(this.PairAt(TwoThreeKeyIndex.Left), sibling.PairAt(TwoThreeKeyIndex.Left), attached, sibling.Children[0], sibling.Children[1]);

                //set new left child and move right child and keys over
                this.ShrinkTo2Node(this.KeyValues[1], newNode, this.Children[2]);
            }
            else if (childIndex == TwoThreeChildIndex.Middle)
            {
                //removing middle child
                //lend left key from this node and key from left sibling (left child)
                var sibling = this.Children[0];
                var newNode = TwoThreeNode<TKey, TValue>.Create3Node(sibling.PairAt(TwoThreeKeyIndex.Left), this.PairAt(TwoThreeKeyIndex.Left), sibling.Children[0], sibling.Children[1], attached);

                //set new left child and move right child and key over
                this.ShrinkTo2Node(this.KeyValues[1], newNode, this.Children[2]);
            }
            else
            {
                //removing right child
                var sibling = this.Children[1];

                //lend right key from parent and key from left sibling (middle child)
                var newNode = TwoThreeNode<TKey, TValue>.Create3Node(sibling.PairAt(TwoThreeKeyIndex.Left), this.PairAt(TwoThreeKeyIndex.Right), sibling.Children[0], sibling.Children[1], attached);
                this.ShrinkTo2Node(this.PairAt(TwoThreeKeyIndex.Left), this.Children[0], newNode);
            }

            return true;
        }

        /// <summary>Creates a merged leaf node with the key from this node and the key in the sibling of the child at <paramref name="childIndex"/>.</summary>
        /// <param name="childIndex">The index of the child being merged.</param>
        /// <returns>The leaf node created from this node and the child at <paramref name="childIndex"/>.</returns>
        public TwoThreeNode<TKey, TValue> MergeChild(TwoThreeChildIndex childIndex)
        {
            int childIdx = ChildIndexToInt(childIndex);
            this.GuardCannotBorrow("Could have borrowed from parent");

            //failed to borrow so merge nodes and move up search path
            var siblingIndex = childIndex == TwoThreeChildIndex.Left ? 1 : 0;
            var sibling = this.Children[siblingIndex];

            sibling.GuardCannotBorrow("Could have borrowed from sibling");

            //if sibling is right-child of parent then its key is the right-key of the merged node
            //otherwise it is the left key
            var mergedNode = new TwoThreeNode<TKey, TValue> { KeyCount = 2 };
            mergedNode.KeyValues[childIdx] = this.KeyValues[0];
            mergedNode.KeyValues[siblingIndex] = sibling.KeyValues[0];

            return mergedNode;
        }

        private void GuardCannotBorrow(string message)
        {
            if (this.CanBorrowKey)
            {
                throw new InvalidOperationException(message);
            }
        }

        private void GuardCanBorrow()
        {
            if (!this.CanBorrowKey) throw new InvalidOperationException("Cannot borrow key");
        }

        private void GuardCanInsert()
        {
            if (this.KeyCount == 2) throw new InvalidOperationException("Cannot insert into full node");
        }

        /// <summary>Gets the key-value pair at the given index in this node.</summary>
        /// <param name="index">The index of the pair to fetch.</param>
        /// <returns>The key-value pair at the given index.</returns>
        /// <exception cref="ArgumentException">If the index is out of range for this node.</exception>
        public KeyValuePair<TKey, TValue> PairAt(TwoThreeKeyIndex index)
        {
            int idx = KeyIndexToInt(index);
            return this.KeyValues[idx];
        }

        /// <summary>Gets the key at the given index in this node.</summary>
        /// <param name="index">The key index to fetch.</param>
        /// <returns>The key at the given index.</returns>
        public TKey KeyAt(TwoThreeKeyIndex index)
        {
            return this.PairAt(index).Key;
        }

        /// <summary>Gets the value at the given index in this node.</summary>
        /// <param name="index">The index of the value to fetch.</param>
        /// <returns>The value at the given index.</returns>
        public TValue ValueAt(TwoThreeKeyIndex index)
        {
            return this.PairAt(index).Value;
        }

        public TwoThreeNode<TKey, TValue> GetChild(TwoThreeChildIndex index)
        {
            int idx = this.ChildIndexToInt(index);
            return this.Children[idx];
        }

        /// <summary>Sets the value associated with the given key index.</summary>
        /// <param name="index">The index of the key to set the associated value for.</param>
        /// <param name="value">The value to associate with the key.</param>
        /// <exception cref="ArgumentException">If the key index is out of range for this node.</exception>
        public void SetValueAt(TwoThreeKeyIndex index, TValue value)
        {
            int idx = KeyIndexToInt(index);
            this.KeyValues[idx] = new KeyValuePair<TKey, TValue>(this.KeyAt(index), value);
        }

        private int KeyIndexToInt(TwoThreeKeyIndex index)
        {
            if (this.KeyCount == 2) return (int)index;
            else
            {
                switch(index)
                {
                    case TwoThreeKeyIndex.Left: return 0;
                    case TwoThreeKeyIndex.Right: throw new ArgumentOutOfRangeException("No right key in 2-node");
                    default: throw new ArgumentException("Invalid key index");
                }
            }
        }

        private int ChildIndexToInt(TwoThreeChildIndex index)
        {
            if (this.KeyCount == 2) return (int)index;
            else
            {
                switch (index)
                {
                    case TwoThreeChildIndex.Left: return 0;
                    case TwoThreeChildIndex.Middle: throw new ArgumentException("No middle child in 2-node");
                    case TwoThreeChildIndex.Right: return 1;
                    default: throw new ArgumentException("Invalid child index");
                }
            }
        }

        private void GuardChildIndex(int childIndex)
        {
            if (childIndex < 0 || childIndex > this.KeyCount) throw new ArgumentOutOfRangeException("childIndex");
        }

        private void AssertIndexInRange(int index)
        {
            if (index < 0 || index >= this.KeyCount) throw new ArgumentOutOfRangeException("index");
        }

        /// <summary>Gets a sequence of key-value pairs in this node.</summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> Pairs
        {
            get
            {
                var pairs = new KeyValuePair<TKey, TValue>[this.KeyCount];
                Array.Copy(this.KeyValues, pairs, this.KeyCount);
                return pairs;
            }
        }

        /// <summary>Creates a leaf 2-node with the given key.</summary>
        /// <param name="kvp">The key-value pair for the leaf.</param>
        /// <returns>A new leaf node with the given key-value pair.</returns>
        public static TwoThreeNode<TKey, TValue> CreateLeaf(KeyValuePair<TKey, TValue> kvp)
        {
            var node = new TwoThreeNode<TKey, TValue>() { KeyCount = 1 };
            node.KeyValues[0] = kvp;
            return node;
        }

        public static TwoThreeNode<TKey, TValue> Create2Node(KeyValuePair<TKey, TValue> keyValue, TwoThreeNode<TKey, TValue> leftChild, TwoThreeNode<TKey, TValue> right)
        {
            var node = new TwoThreeNode<TKey, TValue> { KeyCount = 1 };
            node.KeyValues[0] = keyValue;
            node.Children[0] = leftChild;
            node.Children[1] = right;
            return node;
        }

        public static TwoThreeNode<TKey, TValue> Create3Node(KeyValuePair<TKey, TValue> leftPair, KeyValuePair<TKey, TValue> rightPair, TwoThreeNode<TKey, TValue> leftChild, TwoThreeNode<TKey, TValue> middleChild, TwoThreeNode<TKey, TValue> rightChild)
        {
            var node = new TwoThreeNode<TKey, TValue> { KeyCount = 2 };
            node.KeyValues[0] = leftPair;
            node.KeyValues[1] = rightPair;
            node.Children[0] = leftChild;
            node.Children[1] = middleChild;
            node.Children[2] = rightChild;
            return node;
        }
    }

    public static class TwoThreeNode
    {
        public static TwoThreeNode<TKey, TValue> Create2Node<TKey, TValue>(TKey key, TValue value, TwoThreeNode<TKey, TValue> leftChild, TwoThreeNode<TKey, TValue> rightChild)
        {
            return TwoThreeNode<TKey, TValue>.Create2Node(new KeyValuePair<TKey, TValue>(key, value), leftChild, rightChild);
        }

        public static TwoThreeNode<TKey, TValue> Create3Node<TKey, TValue>(TKey leftKey, TValue leftValue, TKey rightKey, TValue rightValue, TwoThreeNode<TKey, TValue> leftChild, TwoThreeNode<TKey, TValue> middleChild, TwoThreeNode<TKey, TValue> rightChild)
        {
            return TwoThreeNode<TKey, TValue>.Create3Node(new KeyValuePair<TKey, TValue>(leftKey, leftValue), new KeyValuePair<TKey, TValue>(rightKey, rightValue), leftChild, middleChild, rightChild);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> IterateKeyPairsFrom<TKey, TValue>(TwoThreeNode<TKey, TValue> root)
        {
            if (root == null) yield break;

            var parents = new DynamicStack<VisitState<TKey, TValue>>();
            parents.Push(new VisitState<TKey, TValue>(root));

            while (parents.Count > 0)
            {
                var current = parents.Peek();

                switch (current.NextAction)
                {
                    case VisitAction.YieldPair:
                        yield return current.YieldNextPair();
                        break;
                    case VisitAction.VisitChild:
                        var child = current.VisitNextChild();
                        parents.Push(new VisitState<TKey, TValue>(child));
                        break;
                    case VisitAction.Done:
                        parents.Pop();
                        break;
                    default:
                        Debug.Fail("invalid iterator state!");
                        throw new InvalidOperationException();
                }
            }
        }

        private enum VisitAction { YieldPair, VisitChild, Done }
        private class VisitState<TKey, TValue>
        {
            private readonly TwoThreeNode<TKey, TValue> node;
            private TwoThreeKeyIndex keyIndex = TwoThreeKeyIndex.Left;
            private TwoThreeChildIndex childIndex = TwoThreeChildIndex.Left;

            public VisitState(TwoThreeNode<TKey, TValue> node)
            {
                //should visit left child first if one exists otherwise the only thing to do on this node is visit the key
                this.NextAction = node.IsLeaf ? VisitAction.YieldPair : VisitAction.VisitChild;
                this.node = node;
            }

            public VisitAction NextAction { get; private set; }

            public KeyValuePair<TKey, TValue> YieldNextPair()
            {
                GuardState(VisitAction.YieldPair);

                var kvp = this.node.PairAt(this.keyIndex);

                //move to visiting next child if one exists
                //if no children exist then yield all the keys
                //NOTE: Always a right child after the last key unless node is a leaf
                if (this.node.IsLeaf)
                {
                    //yield right key if one exists otherwise done
                    if (this.keyIndex == TwoThreeKeyIndex.Left && this.node.KeyCount == 2)
                    {
                        this.keyIndex = TwoThreeKeyIndex.Right;
                        this.NextAction = VisitAction.YieldPair;
                    }
                    else
                    {
                        this.NextAction = VisitAction.Done;
                    }
                }
                else
                {
                    //get index of child in right-subtree
                    bool visitMiddleChild = (this.keyIndex == TwoThreeKeyIndex.Left && this.node.KeyCount == 2);
                    Require.DebugImplies(visitMiddleChild, this.node.ChildCount == 3, "No middle child for non-leaf 3-node");

                    this.childIndex = visitMiddleChild ? TwoThreeChildIndex.Middle : TwoThreeChildIndex.Right;
                    this.NextAction = VisitAction.VisitChild;
                }

                return kvp;
            }

            public TwoThreeNode<TKey, TValue> VisitNextChild()
            {
                GuardState(VisitAction.VisitChild);

                //calculate next state:
                //finished if this is the right child otherwise move to yielding the key separating this subtree from
                //its right sibling
                switch (this.childIndex)
                {
                    case TwoThreeChildIndex.Left:
                        //visit left key in node
                        this.keyIndex = TwoThreeKeyIndex.Left;
                        this.NextAction = VisitAction.YieldPair;
                        break;

                    case TwoThreeChildIndex.Middle:
                        //NOTE: node should be a 3-node
                        this.keyIndex = TwoThreeKeyIndex.Right;
                        this.NextAction = VisitAction.YieldPair;
                        break;

                    case TwoThreeChildIndex.Right:
                        this.NextAction = VisitAction.Done;
                        break;
                }

                return this.node.GetChild(this.childIndex);
            }

            private void GuardState(VisitAction attempted)
            {
                if (this.NextAction != attempted)
                {
                    var message = string.Format("Cannot perform action {0} in state {1}", attempted, this.NextAction);
                    throw new InvalidOperationException(message);
                }
            }
        }
    }

    public enum TwoThreeChildIndex : byte
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    public enum TwoThreeKeyIndex : byte
    {
        Left = 0,
        Right = 1
    }

    public struct TwoThreeNodeBorrowResult<TKey, TValue>
    {
        public TwoThreeNodeBorrowResult(KeyValuePair<TKey, TValue> borrowedPair, TwoThreeNode<TKey, TValue> borrowedNode)
            : this()
        {
            this.BorrowedPair = borrowedPair;
            this.BorrowedNode = borrowedNode;
        }

        public KeyValuePair<TKey, TValue> BorrowedPair { get; private set; }
        public TwoThreeNode<TKey, TValue> BorrowedNode { get; private set; }
    }

    public struct TwoThreeNodeSplit<TKey, TValue>
    {
        public TwoThreeNodeSplit(TwoThreeNode<TKey, TValue> left, TwoThreeNode<TKey, TValue> right, KeyValuePair<TKey, TValue> middle)
            : this()
        {
            this.Left = left;
            this.Right = right;
            this.MiddleKeyPair = middle;
        }

        public TwoThreeNode<TKey, TValue> Left { get; private set; }
        public TwoThreeNode<TKey, TValue> Right { get; private set; }
        public KeyValuePair<TKey, TValue> MiddleKeyPair { get; private set; }
    }
}
