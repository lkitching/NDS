using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Unbalanced binary search tree.</summary>
    /// <typeparam name="TKey">Type of keys in this tree.</typeparam>
    /// <typeparam name="TValue">Type of values in this key.</typeparam>
    public class BinarySearchTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IComparer<TKey> comp;
        private int count = 0;
        private Node root = null;

        /// <summary>Creates an empty tree with the default comparer for keys.</summary>
        public BinarySearchTree()
            : this(Comparer<TKey>.Default)
        {
        }

        /// <summary>Creates an empty tree with the given kek comparer.</summary>
        /// <param name="comparer">The comparer to use for keys.</param>
        public BinarySearchTree(IComparer<TKey> comparer)
        {
            Contract.Requires(comparer != null);
            this.comp = comparer;
        }

        /// <summary>Finds the value mapped to the given key in this tree if any exists.</summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The value mapped to the given key if it exists in this tree otherwise None.</returns>
        public Maybe<TValue> Get(TKey key)
        {
            Node current = this.root;
            while (current != null)
            {
                int c = this.comp.Compare(key, current.Key);
                if (c == 0)
                {
                    return Maybe.Some(current.Value);
                }
                else if (c < 0)
                {
                    //search left subtree
                    current = current.Left;
                }
                else
                {
                    //search right subtree
                    current = current.Right;
                }
            }

            return Maybe.None<TValue>();
        }

        /// <see cref="IMap{TKey, TValue}.TryAdd"/>
        public bool TryAdd(TKey key, TValue value)
        {
            if (this.root == null)
            {
                this.root = new Node(key, value);
                this.count++;
                return true;
            }
            else
            {
                var current = this.root;
                while (current != null)
                {
                    int comp = this.comp.Compare(key, current.Key);
                    if (comp == 0)
                    {
                        //key exists
                        return false;
                    }
                    else if (comp < 0)
                    {
                        //insert left if null otherwise continue search
                        if (current.Left == null)
                        {
                            current.Left = new Node(key, value);
                            this.count++;
                            return true;
                        }
                        else
                        {
                            current = current.Left;
                        }
                    }
                    else
                    {
                        //insert right if null otherwise continue search
                        if (current.Right == null)
                        {
                            current.Right = new Node(key, value);
                            this.count++;
                            return true;
                        }
                        else
                        {
                            current = current.Right;
                        }
                    }
                }
                return false;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            if (this.root == null)
            {
                this.root = new Node(key, value);
                this.count++;
            }
            else
            {
                var current = this.root;
                while (current != null)
                {
                    var c = this.comp.Compare(key, current.Key);
                    if (c == 0)
                    {
                        current.Value = value;
                        return;
                    }
                    else if (c < 0)
                    {
                        //insert left if null otherwise continue
                        if (current.Left == null)
                        {
                            current.Left = new Node(key, value);
                            this.count++;
                            return;
                        }
                        else
                        {
                            current = current.Left;
                        }
                    }
                    else
                    {
                        if (current.Right == null)
                        {
                            current.Right = new Node(key, value);
                            this.count++;
                            return;
                        }
                        else
                        {
                            current = current.Right;
                        }
                    }
                }
            }
        }

        private static Node DeleteRoot(Node root)
        {
            Debug.Assert(root != null);

            //if left subtree is empty then new root is right subtree
            if (root.Left == null) return root.Right;

            //if right subtree is empty then new root is left subtree
            if (root.Right == null) return root.Left;

            //Both subtrees are non-empty. The smallest element greater than the current root is the left-most
            //node in the right subtree - this should become the new root. The right-subtree of this node
            //should become the new left subtree of that node's parent. There is no left subtree since it is
            //the left-most node in the right subtree.
            var parent = root.Right;
            var current = parent.Left;

            while (current != null)
            {
                parent = current;
                current = current.Left;
            }

            var newRoot = new Node(current.Key, current.Value) { Left = root.Left, Right = root.Right };
            parent.Left = current.Right;
            return newRoot;
        }

        /// <summary>Deletes the given key from this tree.</summary>
        /// <param name="key">The key to delete.</param>
        /// <returns>Whether the key existed in this tree before the delete operaiton.</returns>
        public bool Delete(TKey key)
        {
            Node current = this.root;
            Node parent = null;

            while (current != null)
            {
                int c = this.comp.Compare(key, current.Key);
                if (c == 0)
                {
                    Node newRoot = DeleteRoot(current);
                    if (parent == null)
                    {
                        Debug.Assert(current == this.root);
                        this.root = null;
                    }
                    else if (parent.Left == current) { parent.Left = newRoot; }
                    else
                    {
                        Debug.Assert(parent.Right == current);
                        parent.Right = newRoot;
                    }

                    this.count--;
                    return true;
                }
                else if (c < 0)
                {
                    //search left subtree
                    parent = current;
                    current = current.Left;
                }
                else
                {
                    //search right subtree
                    parent = current;
                    current = current.Right;
                }
            }

            //key not found
            return false;
        }

        /// <summary>Deletes all items from this tree.</summary>
        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        /// <summary>Gets the number of elements in this map.</summary>
        public int Count
        {
            get { return this.count; }
        }

        /// <summary>Performs an in-order traversal of the pairs in this tree.</summary>
        /// <returns>An enumerator for the in-order traversal of this tree.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (this.root == null)
            {
                Debug.Assert(this.Count == 0);
                yield break;
            }

            var parents = new DynamicStack<NodeTraversal>();
            var current = new NodeTraversal(this.root);
            parents.Push(current);

            while (current != null)
            {
                if (current.VisitedRightSubtree)
                {
                    Debug.Assert(current.VisitedLeftSubtree, "Visited right subtree before left");

                    //move back to parent if one exists
                    if (parents.Count > 0)
                    {
                        current = parents.Pop();
                    }
                    else
                    {
                        //reached root
                        break;
                    }
                }
                else if (current.VisitedLeftSubtree)
                {
                    //finished visiting left subtree
                    yield return current.Pair;

                    current.VisitedRightSubtree = true;

                    //visit right subtree if it exists
                    var right = current.Node.Right;
                    if (right != null)
                    {
                        parents.Push(current);
                        current = new NodeTraversal(right);
                    }
                }
                else
                {
                    //visit left subtree
                    current.VisitedLeftSubtree = true;
                    var left = current.Node.Left;

                    if (left != null)
                    {
                        parents.Push(current);
                        current = new NodeTraversal(left);
                    }
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class Node
        {
            public Node(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }

            public TKey Key { get; private set; }
            public TValue Value { get; set; }
            public Node Left { get; set; }
            public Node Right { get; set; }
        }

        private class NodeTraversal
        {
            public NodeTraversal(Node node)
            {
                Contract.Requires(node != null);
                this.Node = node;
            }

            public bool VisitedLeftSubtree { get; set; }
            public bool VisitedRightSubtree { get; set; }
            public Node Node { get; private set; }
            public KeyValuePair<TKey, TValue> Pair
            {
                get { return new KeyValuePair<TKey, TValue>(this.Node.Key, this.Node.Value); }
            }
        }
    }
}
