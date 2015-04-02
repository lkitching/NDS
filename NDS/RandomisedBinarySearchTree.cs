using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>
    /// Binary search tree which inserts new keys randomly along the path to the leaf node they would inserted as in a normal BST. This emulates
    /// a BST with a random insertion order which helps keep the tree balanced.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in this map.</typeparam>
    /// <typeparam name="TValue">The type of values in this map.</typeparam>
    public class RandomisedBinarySearchTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private SizedBSTNode<TKey, TValue> root;
        private readonly Random random;
        private readonly IComparer<TKey> keyComparer;

        /// <summary>Creates a new instance of this class with the default key comparer and random generator.</summary>
        public RandomisedBinarySearchTree() : this(Comparer<TKey>.Default, new Random()) { }

        /// <summary>Creates a new instance of this class with the given key comparer.</summary>
        /// <param name="keyComparer">The comparer to use for the keys in this tree.</param>
        public RandomisedBinarySearchTree(IComparer<TKey> keyComparer) : this(keyComparer, new Random()) { }

        /// <summary>Creates a new instance of this class with the given key comparer and random generator.</summary>
        /// <param name="keyComparer">The comparer to use for the keys in this tree.</param>
        /// <param name="gen">Random generator to use for deciding on root insertion.</param>
        public RandomisedBinarySearchTree(IComparer<TKey> keyComparer, Random gen)
        {
            Contract.Requires(keyComparer != null);
            Contract.Requires(gen != null);

            this.keyComparer = keyComparer;
            this.random = gen;
        }

        /// <see cref="IMap{TKey, TValue}.Get"/>
        public Maybe<TValue> Get(TKey key)
        {
            return BSTNode.Get(this.root, key, this.keyComparer);
        }

        /// <see cref="IMap{TKey, TValue}.TryAdd"/>
        public bool TryAdd(TKey key, TValue value)
        {
            var result = this.AddRec(this.root, key, value);
            this.root = result.Item2;
            return result.Item1;
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            this.root = AssocRec(this.root, key, value).Item2;
        }

        private Tuple<bool, SizedBSTNode<TKey, TValue>> AssocRec(SizedBSTNode<TKey, TValue> root, TKey key, TValue value)
        {
            return AssocOrAddRec(root, key, value, UpdateOp.Assoc);
        }

        private Tuple<bool, SizedBSTNode<TKey, TValue>> AddRec(SizedBSTNode<TKey, TValue> root, TKey key, TValue value)
        {
            return AssocOrAddRec(root, key, value, UpdateOp.Add);
        }

        private enum UpdateOp { Add, Assoc }

        private Tuple<bool, SizedBSTNode<TKey, TValue>> AssocOrAddRec(SizedBSTNode<TKey, TValue> root, TKey key, TValue value, UpdateOp op)
        {
            Contract.Ensures(Contract.Result<Tuple<bool, SizedBSTNode<TKey, TValue>>>().Item2 != null);

            if (root == null)
            {
                return Tuple.Create(true, CreateLeaf(key, value));
            }
            else
            {
                int c = this.keyComparer.Compare(key, root.Key);
                bool insertedChild;

                if (c == 0)
                {
                    //key already exists. If current operation is assoc, update the value of the associated node
                    if (op == UpdateOp.Assoc)
                    {
                        root.Value = value;
                    }
                    insertedChild = false;
                }
                else
                {
                    //perform root insert into subtree with probability
                    //1/(N+1) where N is the count of the current root
                    bool shouldRootInsert = this.random.Next(root.Count + 1) == 0;

                    if(c < 0)
                    {
                        //assoc in left subtree
                        var t = shouldRootInsert ? RootAssocOrAddRec(root.Left, key, value, op) : AssocOrAddRec(root.Left, key, value, op);
                        root.Left = t.Item2;
                        insertedChild = t.Item1;
                    }
                    else
                    {
                        //assoc in right subtree
                        var t = shouldRootInsert ? RootAssocOrAddRec(root.Right, key, value, op) : AssocOrAddRec(root.Right, key, value, op);
                        root.Right = t.Item2;
                        insertedChild = t.Item1;
                    }
                }

                //update count for this node if a child node was inserted
                if (insertedChild) { root.Count++; }
                return Tuple.Create(insertedChild, root);
            }
        }

        private Tuple<bool, SizedBSTNode<TKey, TValue>> RootAssocOrAddRec(SizedBSTNode<TKey, TValue> root, TKey key, TValue value, UpdateOp op)
        {
            Contract.Ensures(Contract.Result<Tuple<bool, SizedBSTNode<TKey, TValue>>>().Item2 != null);

            if (root == null)
            {
                return Tuple.Create(true, CreateLeaf(key, value));
            }
            else
            {
                int c = this.keyComparer.Compare(key, root.Key);
                if (c == 0)
                {
                    //only uppdate if associating since add should fail if key exists
                    if (op == UpdateOp.Assoc)
                    {
                        root.Value = value;
                    }
                    return Tuple.Create(false, root);
                }
                else if (c < 0)
                {
                    var t = RootAssocOrAddRec(root.Left, key, value, op);
                    root.Left = t.Item2;
                    if (t.Item1)
                    {
                        //inserted into left child so update count and rotate right to make left child
                        //root of this node
                        root.Count++;
                        return Tuple.Create(true, RotateSizedRight(root));
                    }
                    else { return Tuple.Create(false, root); }
                }
                else
                {
                    //insert into right subtree
                    var t = RootAssocOrAddRec(root.Right, key, value, op);
                    root.Right = t.Item2;

                    if (t.Item1)
                    {
                        //inserted into right child so update count and rotate left to make right child
                        //root of this node
                        root.Count++;
                        return Tuple.Create(true, RotateSizedLeft(root));
                    }
                    else { return Tuple.Create(false, root); }
                }
            }
        }

        private static SizedBSTNode<TKey, TValue> CreateLeaf(TKey key, TValue value)
        {
            return new SizedBSTNode<TKey, TValue>(key, value) { Count = 1 };
        }

        private static SizedBSTNode<TKey, TValue> RotateSizedLeft(SizedBSTNode<TKey, TValue> parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.Right != null);

            var newParent = BSTNode.RotateLeft(parent);
            Debug.Assert(newParent != parent);

            FixRotatedNodeCounts(newParent, parent);

            return newParent;
        }

        private static SizedBSTNode<TKey, TValue> RotateSizedRight(SizedBSTNode<TKey, TValue> parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.Left != null);

            var newParent = BSTNode.RotateRight(parent);
            Debug.Assert(newParent != parent);

            FixRotatedNodeCounts(newParent, parent);

            return newParent;
        }

        private static void FixRotatedNodeCounts(SizedBSTNode<TKey, TValue> newRoot, SizedBSTNode<TKey, TValue> oldRoot)
        {
            int previousOldRootCount = oldRoot.Count;

            //re-calcualte the size of the old root node since it has a new child node after a rotation
            oldRoot.Count = CalculateNodeSizeFromChildren(oldRoot);

            //NOTE: Total number of nodes in the subtree previously rooted by oldRoot has not changed so
            //size of newRoot should be the previous size of oldRoot
            var calculatedCount = CalculateNodeSizeFromChildren(newRoot);
            Debug.Assert(previousOldRootCount == CalculateNodeSizeFromChildren(newRoot));
            newRoot.Count = previousOldRootCount;
        }

        private static int CalculateNodeSizeFromChildren(SizedBSTNode<TKey, TValue> node)
        {
            return 1 + GetNodeCount(node.Left) + GetNodeCount(node.Right);
        }

        private static void RecalculateNodeCountFromChildren(SizedBSTNode<TKey, TValue> node)
        {
            node.Count = CalculateNodeSizeFromChildren(node);
        }

        private static int GetNodeCount(SizedBSTNode<TKey, TValue> node)
        {
            return node == null ? 0 : node.Count;
        }

        /// <see cref="IMap{TKey, TValue}.Delete"/>
        public bool Delete(TKey key)
        {
            var t = TryDeleteRec(this.root, key);
            bool deleted = t.Item1;
            if (deleted)
            {
                this.root = t.Item2;
            }
            return deleted;
        }

        private Tuple<bool, SizedBSTNode<TKey, TValue>> TryDeleteRec(SizedBSTNode<TKey, TValue> root, TKey key)
        {
            if (root == null) return Tuple.Create(false, root);

            int c = this.keyComparer.Compare(key, root.Key);
            if (c < 0)
            {
                //try to delete from the left subtree. If successful, replace left child of root with updated
                //root in modified subtree.
                var t = TryDeleteRec(root.Left, key);
                bool deleted = t.Item1;
                if (deleted)
                {
                    root.Count--;
                    root.Left = t.Item2;
                }
                return Tuple.Create(deleted, root);
            }
            else if (c > 0)
            {
                //try to delete from the right subtree. If successful, replace right child of root with
                //updated root in modified subtree.
                var t = TryDeleteRec(root.Right, key);
                bool deleted = t.Item1;
                if (deleted)
                {
                    root.Count--;
                    root.Right = t.Item2;
                }
                return Tuple.Create(deleted, root);
            }
            else
            {
                //deleting root - rotate smallest item to the root in the right subtree. This is the node
                //corresponding to the smallest key larger than the root. Since it is the smallest it has
                //no left child (since that would be smaller still) so set the root's left subtree to that
                //node's left subtree and return it.

                //if right child is null then the left subtree should be the new root
                if (root.Right == null)
                {
                    return Tuple.Create(true, root.Left);
                }
                else
                {
                    var newRoot = MoveNthSmallestToRoot(root.Right, 0);
                    newRoot.Left = root.Left;
                    RecalculateNodeCountFromChildren(newRoot);
                    return Tuple.Create(true, newRoot);
                }
            }
        }

        /// <summary>Moves the node with the nth smallest key in the given tree to the root.</summary>
        /// <param name="n">The 0-based index of the key to make the root</param>
        /// <returns>The new root of the tree</returns>
        private static SizedBSTNode<TKey, TValue> MoveNthSmallestToRoot(SizedBSTNode<TKey, TValue> root, int n)
        {
            Contract.Requires(root != null);
            Contract.Requires(n >= 0);
            Contract.Requires(n < root.Count - 1);

            //find the number of nodes in the left subtree - if it is greater than n then the nth smallest is in
            //the left subtree so continue the search there. If it is less then n, there are (n-leftCount-1) smaller
            //nodes in the right subtree so search for the (n-left-1)th-smallest node there. If n == leftCount
            //the root is the nth smallest node in the tree. Once the nth-smallest node has been found it should
            //be rotated up through the tree until it is at the root.
            int leftCount = GetNodeCount(root.Left);
            if (leftCount > n)
            {
                root.Left = MoveNthSmallestToRoot(root.Left, n);
                return RotateSizedRight(root);
            }
            else if (leftCount < n)
            {
                int remaining = n - leftCount - 1;
                Debug.Assert(remaining > 0);
                root.Right = MoveNthSmallestToRoot(root.Right, n);
                return RotateSizedLeft(root);
            }
            else { return root; }
        }

        /// <see cref="IMap{TKey, TValue}.Clear"/>
        public void Clear()
        {
            this.root = null;
        }

        /// <see cref="IMap{TKey, TValue}.Count"/>
        public int Count
        {
            get { return GetNodeCount(this.root); }
        }

        /// <summary>Gets an enumerator for the pairs in this map.</summary>
        /// <returns>An enumerator for the key-value pairs in this map.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return BSTTraversal.InOrder(this.root).Select(n => n.ToKeyValuePair()).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
