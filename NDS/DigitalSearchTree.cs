using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NDS
{
    /// <summary>
    /// Search tree which is structured based on the binary structure of the contained keys. The left child of a node at depth N in the tree 
    /// has a 0 bit at position N in the associated key while the right child has a 1 bit. All keys are stored at some path in the tree which
    /// represents a prefix of the binary key digits.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the tree.</typeparam>
    /// <typeparam name="TValue">The type of values in the tree.</typeparam>
    public class DigitalSearchTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IBitAddressable<TKey> keyAddr;
        private readonly IEqualityComparer<TKey> keyComp;
        private Node root;
        private int count;

        /// <summary>Creates a new instance of this class with the default key comparer.</summary>
        /// <param name="keyAddressable">Retrives individual bits within keys.</param>
        public DigitalSearchTree(IBitAddressable<TKey> keyAddressable): this(keyAddressable, EqualityComparer<TKey>.Default) { }

        /// <summary>Creates a new instance of this class.</summary>
        /// <param name="keyAddressable">Retrives individual bits within keys.</param>
        /// <param name="keyComparer">Compares keys for equality.</param>
        public DigitalSearchTree(IBitAddressable<TKey> keyAddressable, IEqualityComparer<TKey> keyComparer)
        {
            this.keyAddr = keyAddressable;
            this.keyComp = keyComparer;
        }

        /// <summary>Returns the number of items in this tree.</summary>
        public int Count
        {
            get { return this.count; }
        }

        public void Assoc(TKey key, TValue value)
        {
            var context = Search(key);
            if(context.Found)
            {
                context.MatchingNode.Value = value;
            }
            else
            {
                ApplyInsert(key, value, context);
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            var context = Search(key);
            if (context.Found) return false;
            else
            {
                ApplyInsert(key, value, context);
                return true;
            }
        }

        private void ApplyInsert(TKey key, TValue value, SearchContext context)
        {
            var searchPath = context.SearchPath;
            if(searchPath.Count == 0)
            {
                //inserting into empty tree
                Debug.Assert(this.root == null);
                this.root = new Node(key, value);
            }
            else
            {
                var parent = searchPath[searchPath.Count - 1].Node;

                //branch at depth N in the tree is based on bit[N] within the inserting key
                //parent node should not already contain a child in the associated position (or the
                //search for the key could have continued)
                var bit = this.keyAddr.GetBit(key, searchPath.Count - 1);
                Debug.Assert(parent.GetChild(bit) == null, "Parent should not contain child at insert position.");
                parent.SetChild(bit, new Node(key, value));
            }

            this.count++;
        }

        public void Clear()
        {
            this.root = null;
            this.count = 0;
        }

        public bool Delete(TKey key)
        {
            var context = Search(key);
            if (context.Found)
            {
                ApplyDelete(key, context);
                return true;
            }
            else return false;
        }

        void ApplyDelete(TKey key, SearchContext context)
        {
            var searchPath = context.SearchPath;
            var removingNode = context.MatchingNode;

            if(searchPath.Count == 0)
            {
                //removing root
                Debug.Assert(context.MatchingNode == this.root);
                this.root = null;
            }
            else
            {
                var parentBranch = searchPath[searchPath.Count - 1];
                var parent = parentBranch.Node;

                switch(removingNode.ChildCount)
                {
                    case 0:
                        //removing node is leaf so just remove it from the parent
                        parentBranch.SetChildNode(null);
                        break;
                    case 1:
                        //replace removing node with single child in parent
                        var child = removingNode.Left ?? removingNode.Right;
                        parentBranch.SetChildNode(child);
                        break;
                    case 2:
                        //bit sequence representing the search path is a prefix of both the removing node and all of descendants
                        //find any leaf in the subtree rooted at the removing node and swap it in the parent
                        //arbitrarily choose the left-most leaf in the subtree to replace the removing node
                        Node leafParent = parent.Left;
                        Debug.Assert(leafParent != null);

                        Node leaf = leafParent.Left;
                        while(leaf != null)
                        {
                            leafParent = leaf;
                            leaf = leafParent.Left;
                        }

                        //remove leaf from its parent
                        leafParent.Left = null;

                        //make removing node's left child the left child of the leaf (unless it is the leaf itself!)
                        if(removingNode.Left != leaf)
                        {
                            removingNode.Left = leaf.Left;
                        }

                        //swap leaf node with parent
                        parentBranch.SetChildNode(leaf);
                        break;
                }
            }

            --this.count;
        }

        public Maybe<TValue> Get(TKey key)
        {
            var context = Search(key);
            return context.Found ? Maybe.Some(context.MatchingNode.Value) : Maybe.None<TValue>();
        }

        private SearchContext Search(TKey key)
        {
            var searchPath = new ArrayList<SearchBranch<Node>>();

            Node current = this.root;
            for (int i = 0; i < this.keyAddr.NumBits && current != null; ++i)
            {
                if (this.keyComp.Equals(key, current.Key)) return new SearchContext(current, searchPath);
                else
                {
                    Bit keyBit = this.keyAddr.GetBit(key, i);
                    searchPath.Add(new SearchBranch<Node>(current, BitSearchDirection(keyBit)));
                    current = current.GetChild(keyBit);
                }
            }

            //not found
            return new SearchContext(null, searchPath);
        }

        private static BranchDirection BitSearchDirection(Bit b)
        {
            return b == Bit.Zero ? BranchDirection.Left : BranchDirection.Right;
        }

        private struct SearchContext : IBSTSearchContext<Node>
        {
            public SearchContext(Node matchingNode, ArrayList<SearchBranch<Node>> searchPath): this()
            {
                this.MatchingNode = matchingNode;
                this.SearchPath = searchPath;
            }

            public bool Found
            {
                get { return this.MatchingNode != null; }
            }

            public Node MatchingNode { get; private set; }

            public ArrayList<SearchBranch<Node>> SearchPath { get; private set; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return BSTTraversal.InOrder(this.root).Select(n => new KeyValuePair<TKey, TValue>(n.Key, n.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private class Node : IBinaryNode<Node>
        {
            private readonly Node[] children = new Node[2];

            public Node(TKey key, TValue value)
            {
                this.Key = key;
                this.Value = value;
            }

            public TKey Key { get; }
            public TValue Value { get; set; }

            public Node GetChild(Bit b) { return this.children[(int)b]; }
            public void SetChild(Bit b, Node n) { this.children[(int)b] = n; }

            public Node Left
            {
                get { return this.GetChild(Bit.Zero); }
                set { this.SetChild(Bit.Zero, value); }
            }

            public Node Right
            {
                get { return this.GetChild(Bit.One); }
                set { this.SetChild(Bit.One, value); }
            }

            public int ChildCount
            {
                get { return (this.Left == null ? 0 : 1) + (this.Right == null ? 0 : 1); }
            }

            public bool IsLeaf
            {
                get { return children[0] == null && children[1] == null; }
            }
        }
    }
}
