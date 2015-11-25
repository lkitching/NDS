using System;
using System.Diagnostics;

namespace NDS
{
    /// <summary>Van Emde Boas tree.</summary>
    public class VEBTree
    {
        private readonly VEBTree[] children;
        private readonly byte height;

        /// <summary>Creates a new empty tree.</summary>
        public VEBTree()
            : this(5)
        {
        }

        private VEBTree(byte height)
        {
            this.children = new VEBTree[GetChildCount(height)];

            if (height > 0)
            {
                this.Summary = new VEBTree((byte)(height - 1));
            }
            this.height = height;
            this.SetEmpty();
        }

        private static int GetChildCount(byte height)
        {
            Debug.Assert(height >= 0);
            var childCounts = new[] { 0, 2, 4, 16, 256, 65536 };
            return childCounts[height];
        }

        private uint MinValue { get; set; }
        private uint MaxValue { get; set; }

        /// <summary>Whether this tree is empty.</summary>
        public bool IsEmpty
        {
            get { return MinValue > MaxValue; }
        }

        /// <summary>Whether this tree contains the given value.</summary>
        /// <param name="i">The value to search for.</param>
        /// <returns>Whether this tree contains the value.</returns>
        public bool Contains(uint i)
        {
            if (i < this.MinValue || i > this.MaxValue) return false;
            else if (i == this.MinValue || i == this.MaxValue) return true;
            else
            {
                //find child tree which might contains item
                var childLocation = FindChildLocation(i);
                VEBTree childTree = this.Children[childLocation.ChildIndex];

                return childTree != null && childTree.Contains(childLocation.Value);
            }
        }

        /// <summary>Inserts a value into this tree.</summary>
        /// <param name="i">The value to insert.</param>
        public void Insert(uint i)
        {
            if (this.IsEmpty)
            {
                this.MinValue = this.MaxValue = i;
            }
            else if (this.MinValue == this.MaxValue)
            {
                //tree contains only one element so adjust min or max depending on i
                if (i < this.MinValue)
                {
                    this.MinValue = i;
                }
                else if (i > this.MaxValue)
                {
                    this.MaxValue = i;
                }
            }
            else if (i < this.MinValue)
            {
                //insert the current min into its corresponding child tree
                //and update new min
                InsertIntoChild(this.MinValue);
                this.MinValue = i;
            }
            else if (i > this.MaxValue)
            {
                //insert the current max into the corresponding child tree
                //and update new max
                this.InsertIntoChild(this.MaxValue);
                this.MaxValue = i;
            }
            else if (i != this.MinValue && i != this.MaxValue)
            {
                Debug.Assert(i > this.MinValue, "i <= min");
                Debug.Assert(i < this.MaxValue, "i >= max");

                InsertIntoChild(i);
            }
            else
            {
                //i already exists in this tree
                Debug.Assert(i == this.MinValue || i == this.MaxValue);
            }
        }

        private void InsertIntoChild(uint i)
        {
            //insert i into the corresponding child tree
            var childLocation = FindChildLocation(i);
            VEBTree child = FindOrCreateChild(childLocation);

            child.Insert(childLocation.Value);

            //update summary since child tree not empty
            this.Summary.Insert((uint)childLocation.ChildIndex);
        }

        private VEBTree FindOrCreateChild(ChildLocation location)
        {
            var child = this.Children[location.ChildIndex];
            if (child == null)
            {
                child = CreateChildTree();
                this.Children[location.ChildIndex] = child;
            }
            return child;
        }

        private VEBTree CreateChildTree()
        {
            return new VEBTree((byte)(this.height - 1));
        }

        /// <summary>Removes a value from this tree if it exists.</summary>
        /// <param name="i">The value to remove.</param>
        /// <returns>True if the value existed before this operation, otherwise false.</returns>
        public bool Delete(uint i)
        {
            if (this.IsEmpty) return false;
            else if (i < this.MinValue || i > this.MaxValue) return false;
            else if (MinValue == MaxValue)
            {
                //only one element in this tree. see if it is the element to delete
                if (i == MinValue)
                {
                    //i is the only element in this tree so set empty
                    SetEmpty();
                    return true;
                }
                else return false;
            }
            else if (i == MinValue)
            {
                //the minimum tree element is being removed so the new min is the second-smallest item
                if (this.IsLeaf)
                {
                    //this node has no children so the second-smallest item is the maximum
                    this.MinValue = this.MaxValue;
                }
                else
                {
                    //this is the minimum item in the minimum child tree
                    //if no child trees are non-empty then the second-smallest item is the tree max
                    var minTreeIndex = this.Summary.FindMin();
                    if (minTreeIndex.HasValue)
                    {
                        int childIndex = (int)minTreeIndex.Value;
                        var minChild = this.children[childIndex];
                        uint secondMin = ReconstructChildValue(childIndex, minChild.MinValue);

                        //remove the second-min from the child tree and replace the old min in this tree
                        bool removed = minChild.Delete(minChild.MinValue);
                        Debug.Assert(removed, "Failed to remove minimum from tree");

                        //if the child tree is now empty it needs to be removed from the summary
                        if (minChild.IsEmpty)
                        {
                            bool removedFromSummary = this.Summary.Delete((uint)childIndex);
                            Debug.Assert(removedFromSummary, "Failed to remove child index from summary");
                        }

                        this.MinValue = secondMin;
                    }
                    else
                    {
                        //second-smallest value is the max so this tree contains only two values
                        //set min to max
                        MinValue = MaxValue;
                    }
                }
                return true;
            }
            else if (i == MaxValue)
            {
                //the maximum tree element is being removed so the new max is the second-largest item
                if (this.IsLeaf)
                {
                    //this node contains no children so the second-largest value is the minimum
                    this.MaxValue = this.MinValue;
                }
                else
                {
                    //second-largest value is the maximum item in the maximum child tree
                    //if no child trees are non-empty then the second-largest item is the tree min
                    var maxTreeIndex = this.Summary.FindMax();
                    if (maxTreeIndex.HasValue)
                    {
                        int maxChildIndex = (int)maxTreeIndex.Value;
                        VEBTree maxChild = this.Children[maxChildIndex];
                        uint secondMax = ReconstructChildValue(maxChildIndex, maxChild.MaxValue);

                        //remove the second-max item from its containing subtree
                        bool removed = maxChild.Delete(maxChild.MaxValue);
                        Debug.Assert(removed, "Failed to remove max item from containing tree");

                        //if the tree containing the old second-largest item is now empty, remove it from the summary
                        if (maxChild.IsEmpty)
                        {
                            bool removedFromSummary = this.Summary.Delete((uint)maxChildIndex);
                            Debug.Assert(removedFromSummary, "Failed to remove child index from summary");
                        }

                        this.MaxValue = secondMax;
                    }
                    else
                    {
                        //no max child tree exists so the second-largest item in this tree is the minimum
                        //set max to min
                        MaxValue = MinValue;
                    }
                }

                return true;
            }
            else
            {
                //find the child tree i could be contained in and try delete it
                var childLocation = FindChildLocation(i);
                var childTree = this.children[childLocation.ChildIndex];

                if (childTree == null) return false;
                bool removed = childTree.Delete(childLocation.Value);

                if (removed && childTree.IsEmpty)
                {
                    bool removedFromSummary = this.Summary.Delete((uint)childLocation.ChildIndex);
                    Debug.Assert(removedFromSummary, "Failed to remove from summary");
                }

                return removed;
            }
        }

        /// <summary>Finds the smallest item in this tree greater than the given value if one exists.</summary>
        /// <param name="i">The value to find the smallest value greater than.</param>
        /// <returns>The smallest value in this tree greater than the given value if one exists, otherwise None.</returns>
        public Maybe<uint> FindNext(uint i)
        {
            if (this.IsEmpty || i >= this.MaxValue) return Maybe.None<uint>();
            else if (i < this.MinValue) return Maybe.Some(this.MinValue);

            if (this.IsLeaf)
            {
                //if this is the bottom level of the tree then there are at most two elements
                //if i < Min then Min is the next value, otherwise it is Max
                uint next = i < MinValue ? MinValue : MaxValue;
                return Maybe.Some(next);
            }

            var childLocation = FindChildLocation(i);
            var child = this.Children[childLocation.ChildIndex];

            if (IsNonEmpty(child) && childLocation.Value < child.MaxValue)
            {
                //successor is contained in the same child tree as i
                //find it in the child node and then re-construct the value
                Maybe<uint> childNext = child.FindNext(childLocation.Value);
                Debug.Assert(childNext.HasValue, "Should find successor in child segment");

                uint v = ReconstructChildValue(childLocation.ChildIndex, childNext.Value);
                return Maybe.Some(v);
            }
            else
            {
                //find the successor child to the child containing i
                //if it exists the next value is the min value of that child tree
                //if it does not exist then the tree max must be the successor
                Maybe<uint> maybeChildSuccessorIndex = this.Summary.FindNext((uint)childLocation.ChildIndex);
                if (maybeChildSuccessorIndex.HasValue)
                {
                    int childSuccessorIndex = (int)maybeChildSuccessorIndex.Value;
                    var successorChild = this.Children[childSuccessorIndex];
                    uint v = ReconstructChildValue(childSuccessorIndex, successorChild.MinValue);
                    return Maybe.Some(v);
                }
                else return Maybe.Some(this.MaxValue);
            }
        }

        /// <summary>Finds the greatest item in this tree smaller than the given value if one exists.</summary>
        /// <param name="i">The value to find the greatest value smaller than.</param>
        /// <returns>The greatest value in this tree smaller than the given value if one exists, otherwise None.</returns>
        public Maybe<uint> FindPrevious(uint i)
        {
            if (this.IsEmpty || i <= this.MinValue) return Maybe.None<uint>();
            else if (i > this.MaxValue) return Maybe.Some(this.MaxValue);

            if (this.IsLeaf)
            {
                //if this is the bottom level of the tree then there is at most a min and max value
                //if i > Max then Max is the previous value, otherwise it is Min
                uint previousValue = i > MaxValue ? MaxValue : MinValue;
                return Maybe.Some(previousValue);
            }

            //find the child tree containing i
            var childLocation = FindChildLocation(i);
            var child = this.Children[childLocation.ChildIndex];

            if (IsNonEmpty(child) && childLocation.Value > child.MinValue)
            {
                //predecessor for i is in the same child tree as i itself
                Maybe<uint> childPrevious = child.FindPrevious(childLocation.Value);
                Debug.Assert(childPrevious.HasValue, "Should find previous value in child");

                uint value = ReconstructChildValue(childLocation.ChildIndex, childPrevious.Value);
                return Maybe.Some(value);
            }
            else
            {
                //find the predecessor child to the child tree containing i
                //if it exists the predecessor to i is the max of that tree
                //if it does not exist the predecessor to i must be the tree min
                Maybe<uint> maybePredecessorChildIndex = this.Summary.FindPrevious((uint)childLocation.ChildIndex);
                if (maybePredecessorChildIndex.HasValue)
                {
                    var predecessorChildIndex = (int)maybePredecessorChildIndex.Value;
                    var predecessorChild = this.Children[predecessorChildIndex];
                    var predecessorValue = ReconstructChildValue(predecessorChildIndex, predecessorChild.MaxValue);
                    return Maybe.Some(predecessorValue);
                }
                else return Maybe.Some(this.MinValue);
            }
        }

        private bool IsLeaf
        {
            get { return this.Children.Length == 0; }
        }

        private void SetEmpty()
        {
            this.MaxValue = 0;
            this.MinValue = uint.MaxValue;
        }

        /// <summary>Gets the minimum value in this tree if one exists.</summary>
        public Maybe<uint> FindMin()
        {
            return this.IsEmpty ? Maybe.None<uint>() : Maybe.Some(this.MinValue);
        }

        /// <summary>Gets the maximum value in this tree if one exists.</summary>
        public Maybe<uint> FindMax()
        {
            return this.IsEmpty ? Maybe.None<uint>() : Maybe.Some(this.MaxValue);
        }

        private VEBTree Summary { get; set; }

        private VEBTree[] Children
        {
            get { return this.children; }
        }

        private uint ReconstructChildValue(int childIndex, uint value)
        {
            uint childBase = (uint)(childIndex * this.children.Length);
            return childBase + value;
        }

        private static bool IsNonEmpty(VEBTree t)
        {
            return t != null && !t.IsEmpty;
        }

        private ChildLocation FindChildLocation(uint item)
        {
            int childIndex = (int)(item / this.Children.Length);
            var value = (uint)(item % this.Children.Length);

            Debug.Assert(childIndex >= 0 && childIndex < this.Children.Length);
            return new ChildLocation(childIndex, value);
        }

        private struct ChildLocation
        {
            public ChildLocation(int childIndex, uint value)
                : this()
            {
                this.ChildIndex = childIndex;
                this.Value = value;
            }

            public int ChildIndex { get; private set; }
            public uint Value { get; private set; }
        }
    }
}
