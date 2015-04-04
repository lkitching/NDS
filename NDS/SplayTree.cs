using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace NDS
{
    /// <summary>
    /// Splay trees are binary search trees which moved the accessed nodes to the root of the tree when adding/updating or querying.
    /// This makes repeated access to the same key more efficient than in a regular binary search tree.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in this map.</typeparam>
    /// <typeparam name="TValue">The type of values in this map.</typeparam>
    public class SplayTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IComparer<TKey> keyComparer;
        private BSTNode<TKey, TValue> root;
        private int count;

        /// <summary>Creates a new tree with the default key comparer.</summary>
        public SplayTree() : this(Comparer<TKey>.Default) { }

        /// <summary>Creates a new tree with the given key comparer.</summary>
        /// <param name="keyComparer">Comparer for keys in this tree.</param>
        public SplayTree(IComparer<TKey> keyComparer)
        {
            Contract.Requires(keyComparer != null);
            this.keyComparer = keyComparer;
        }

        /// <summary>
        /// Gets the value associated with the given key in this tree. If the key exists then after this operation the corresponding node
        /// will be at the root of the tree.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The value associated with <paramref name="key"/> if it exists.</returns>
        public Maybe<TValue> Get(TKey key)
        {
            var result = SplayGet(this.root, key);
            this.root = result.Item2;

            //searched-for key should be at the root if found
            //HasVaulue => comp(key, root.Key) == 0
            Debug.Assert(!result.Item1.HasValue || this.keyComparer.Compare(key, this.root.Key) == 0, "Accessed node not at root after Get");

            return result.Item1;
        }

        private Tuple<Maybe<TValue>, BSTNode<TKey, TValue>> SplayGet(BSTNode<TKey, TValue> root, TKey key)
        {
            if (root == null) { return NotFound(root); }

            int c = this.keyComparer.Compare(key, root.Key);
            if (c == 0)
            {
                return Tuple.Create(Maybe.Some(root.Value), root);
            }
            else if (c < 0)
            {
                //search left subtree
                var leftChild = root.Left;
                if (leftChild == null) { return NotFound(root); }

                int lc = this.keyComparer.Compare(key, leftChild.Key);
                if (lc == 0) { return Tuple.Create(Maybe.Some(leftChild.Value), leftChild); }
                else if (lc < 0)
                {
                    //left-left path in tree
                    //if key exists in subtree then rotate 'top down' from the root so the node with the
                    //matching key is at the root.
                    var result = SplayGet(leftChild.Left, key);
                    leftChild.Left = result.Item2;

                    if (result.Item1.HasValue)
                    {
                        var tmp = BSTNode.RotateRight(root);
                        var newRoot = BSTNode.RotateRight(tmp);
                        return Tuple.Create(result.Item1, newRoot);
                    }
                    else { return NotFound(root); }
                }
                else
                {
                    //left-right path
                    //rotate 'bottom up' if key is found in subtree
                    var result = SplayGet(leftChild.Right, key);
                    leftChild.Right = result.Item2;

                    if (result.Item1.HasValue)
                    {
                        root.Left = BSTNode.RotateLeft(leftChild);
                        var newRoot = BSTNode.RotateRight(root);
                        return Tuple.Create(result.Item1, newRoot);
                    }
                    else { return NotFound(root); }
                }
            }
            else
            {
                //search right subtree
                var rightChild = root.Right;
                if (rightChild == null) { return NotFound(root); }

                int rc = this.keyComparer.Compare(key, rightChild.Key);
                if (rc == 0)
                {
                    return Tuple.Create(Maybe.Some(rightChild.Value), rightChild);
                }
                else if (rc < 0)
                {
                    //right-left path
                    //rotate 'bottom up' if key is found in subtree
                    var result = SplayGet(rightChild.Left, key);
                    rightChild.Left = result.Item2;

                    if (result.Item1.HasValue)
                    {
                        root.Right = BSTNode.RotateRight(rightChild);
                        var newRoot = BSTNode.RotateLeft(root);
                        return Tuple.Create(result.Item1, newRoot);
                    }
                    else { return NotFound(root); }
                }
                else
                {
                    //right-right path
                    //rotate 'top down' from the root if key is found in subtree
                    var result = SplayGet(rightChild.Right, key);
                    rightChild.Right = result.Item2;

                    if (result.Item1.HasValue)
                    {
                        var tmp = BSTNode.RotateLeft(root);
                        var newRoot = BSTNode.RotateLeft(tmp);
                        return Tuple.Create(result.Item1, newRoot);
                    }
                    else { return NotFound(root); }
                }
            }
        }

        private static Tuple<Maybe<TValue>, BSTNode<TKey, TValue>> NotFound(BSTNode<TKey, TValue> root)
        {
            return Tuple.Create(Maybe.None<TValue>(), root);
        }

        /// <summary>
        /// Adds a key to this map if it does not exist. If a new node is added it will be at the root
        /// after this operation.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>Whether the key was added. If false is returned the key already existed in this map prior to this operation.</returns>
        /// <see cref="IMap{TKey, TValue}"/>
        public bool TryAdd(TKey key, TValue value)
        {
            return AddOrAssoc(key, value, UpdateOp.Add);
        }

        /// <summary>
        /// Associates a value with the given key in this map. After the operation the node containing
        /// <paramref name="key"/> will be at the root.
        /// </summary>
        /// <param name="key">The key to add or update.</param>
        /// <param name="value">The value to associate with <paramref name="key"/>.</param>
        public void Assoc(TKey key, TValue value)
        {
            AddOrAssoc(key, value, UpdateOp.Assoc);
        }

        private bool AddOrAssoc(TKey key, TValue value, UpdateOp op)
        {
            var result = SplayAddOrAssoc(this.root, key, value, op);

            this.root = result.NewRoot;

            //update count if item inserted
            if (result.AddedNode)
            {
                this.count++;
                Debug.Assert(this.root != null);
                Debug.Assert(this.keyComparer.Compare(key, this.root.Key) == 0, "Accessed node not at root");
            }
            return result.AddedNode;
        }

        /// <see cref="IMap{TKey, TValue}.Clear"/>
        public bool Delete(TKey key)
        {
            var result = BSTNode.Delete(this.root, key, this.keyComparer);
            this.root = result.Item2;
            bool deleted = result.Item1;

            if (deleted) { this.count--; }
            return deleted;
        }

        private UpdateResult SplayAddOrAssoc(BSTNode<TKey, TValue> root, TKey key, TValue value, UpdateOp op)
        {
            if (root == null) { return UpdateResult.Created(BSTNode.Create(key, value)); }

            int c = this.keyComparer.Compare(key, root.Key);
            if (c == 0)
            {
                if (op == UpdateOp.Assoc)
                {
                    root.Value = value;
                }
                return UpdateResult.Existing(root);
            }
            else if (c < 0)
            {
                var leftChild = root.Left;

                //make current root right child of new node if there is no right subtree
                if (leftChild == null) return UpdateResult.Created(BSTNode.Create(key, value, right: root));

                int lc = this.keyComparer.Compare(key, leftChild.Key);

                if (lc == 0)
                {
                    //only update value in left child if associating
                    if (op == UpdateOp.Assoc)
                    {
                        leftChild.Value = value;
                    }

                    //rotate accessed node up to root
                    var newRoot = BSTNode.RotateRight(root);
                    return UpdateResult.Existing(newRoot);
                }
                else if (lc < 0)
                {
                    //left-left path in tree. 
                    //add/assoc into left-left subtree then rotate 'top down' from the root
                    var assocResult = SplayAddOrAssoc(leftChild.Left, key, value, op);
                    leftChild.Left = assocResult.NewRoot;
                    var tmp = BSTNode.RotateRight(root);
                    var newRoot = BSTNode.RotateRight(tmp);
                    return assocResult.WithRoot(newRoot);
                }
                else
                {
                    //left-right tree
                    //add/assoc into left-right subtree then rotate 'bottom up' from the inserted/updated node
                    var assocResult = SplayAddOrAssoc(leftChild.Right, key, value, op);
                    leftChild.Right = assocResult.NewRoot;
                    root.Left = BSTNode.RotateLeft(leftChild);
                    var newRoot = BSTNode.RotateRight(root);
                    return assocResult.WithRoot(newRoot);
                }
            }
            else
            {
                var rightChild = root.Right;

                //make current root left child of new node if there is no right subtree
                if (rightChild == null) return UpdateResult.Created(BSTNode.Create(key, value, left: root));

                int rc = this.keyComparer.Compare(key, rightChild.Key);
                if (rc == 0)
                {
                    //only update value in right child if associating
                    if (op == UpdateOp.Assoc)
                    {
                        rightChild.Value = value;
                    }

                    //rotate accessed node up to the root
                    var newRoot = BSTNode.RotateLeft(root);
                    return UpdateResult.Existing(newRoot);
                }
                else if (rc < 0)
                {
                    //right-left tree
                    //add/assoc into right-left tree then rotate 'bottom up' from the inserted/updated node
                    var assocResult = SplayAddOrAssoc(rightChild.Left, key, value, op);
                    rightChild.Left = assocResult.NewRoot;
                    root.Right = BSTNode.RotateRight(rightChild);
                    var newRoot = BSTNode.RotateLeft(root);
                    return assocResult.WithRoot(newRoot);
                }
                else
                {
                    //right-right path in tree
                    //add/assoc into right-right subtree and rotate 'top down' from the current root
                    var assocResult = SplayAddOrAssoc(rightChild.Right, key, value, op);
                    rightChild.Right = assocResult.NewRoot;
                    var tmp = BSTNode.RotateLeft(root);
                    var newRoot = BSTNode.RotateLeft(tmp);
                    return assocResult.WithRoot(newRoot);
                }
            }
        }

        private enum UpdateOp { Add, Assoc }

        private class UpdateResult
        {
            public bool AddedNode { get; private set; }
            public BSTNode<TKey, TValue> NewRoot { get; private set; }

            public UpdateResult WithRoot(BSTNode<TKey, TValue> root)
            {
                return new UpdateResult { AddedNode = this.AddedNode, NewRoot = root };
            }

            public static UpdateResult Created(BSTNode<TKey, TValue> newNode)
            {
                return new UpdateResult { AddedNode = true, NewRoot = newNode };
            }

            public static UpdateResult Existing(BSTNode<TKey, TValue> existingNode)
            {
                return new UpdateResult { AddedNode = false, NewRoot = existingNode };
            }
        }

        /// <summary>Clears all pairs from this map.</summary>
        /// <see cref="IMap{TKey, TValue}.Clear"/>
        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        /// <summary>Gets the number of elements in this tree.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Gets an enumerator for the key-value pairs in this map.</summary>
        /// <returns>An enumerator for the pairs in this map.</returns>
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
