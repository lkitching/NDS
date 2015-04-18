using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Tree with nodes which can have 1 or 2 keys and 2 or 3 (or no) children.</summary>
    /// <typeparam name="TKey">Key type for the tree.</typeparam>
    /// <typeparam name="TValue">Value type for the tree.</typeparam>
    public class TwoThreeTree<TKey, TValue> : IMap<TKey, TValue>
    {
        private readonly IComparer<TKey> comparer;
        private TwoThreeNode<TKey, TValue> root;
        private int count;

        /// <summary>Creates a new empty tree with the default key comparer.</summary>
        public TwoThreeTree() : this(Comparer<TKey>.Default) { }

        /// <summary>Creates a new empty tree with the given key comparer.</summary>
        /// <param name="keyComparer">Comparer for the keys in this tree.</param>
        public TwoThreeTree(IComparer<TKey> keyComparer)
        {
            Contract.Requires(keyComparer != null);
            this.comparer = keyComparer;
            this.count = 0;
        }

        /// <see cref="IMap{TKey, TValue}.Get"/>
        public Maybe<TValue> Get(TKey key)
        {
            var search = SearchFor(this.root, key, this.comparer);
            if (search.Found)
            {
                var node = search.Parent;
                return Maybe.Some(node.ValueAt(search.KeyIndex));
            }
            else return Maybe.None<TValue>();
        }

        /// <see cref="IMap{TKey, TValue}.TryAdd"/>
        public bool TryAdd(TKey key, TValue value)
        {
            var search = SearchFor(key);
            if (search.Found) return false;
            else
            {
                ApplyInsert(search, key, value);
                return true;
            }
        }

        /// <see cref="IMap{TKey, TValue}.Assoc"/>
        public void Assoc(TKey key, TValue value)
        {
            var search = SearchFor(key);
            ApplyInsert(search, key, value);
        }

        /// <see cref="IMap{TKey, TValue}.Delete"/>
        public bool Delete(TKey key)
        {
            var search = SearchFor(key);
            if (search.Found)
            {
                DeleteNode(search);
                this.count--;
                return true;
            }
            else return false;
        }

        private void DeleteNode(SearchContext search)
        {
            Debug.Assert(search.Found, "Deleting from failed search");
            TwoThreeNode<TKey, TValue> parent = search.Parent;

            if (! parent.IsLeaf)
            {
                //keys are deleted from the leaves and changed propagated up the tree
                //find the in-order successor of the key to delete (smallest key larger than this one)
                //and swap the keys so the one to remove is in a leaf node.
                //delete from this leaf with the new search context
                var currentKeyIndex = search.KeyIndex;
                search = FindSuccessor(search);

                //NOTE: Successor must exist since if this was the largest key in the tree it would be the
                //right-most leaf node
                Debug.Assert(search.Found, "Failed to find successor");

                //swap keys
                search.Parent.SwapPairWith(search.KeyIndex, parent, currentKeyIndex);
            }

            DeleteFromLeaf(search);
        }

        private static SearchContext FindSuccessor(SearchContext initialSearch)
        {
            var parent = initialSearch.Parent;
            var searchPath = initialSearch.SearchPath;
            
            //first move to the right subtree for the found key
            //NOTE: mutating the original search path in place but it doesn't need to be reused
            Debug.Assert(initialSearch.Found, "Cannot find successor of failed search");

            //find index of right-child for found key
            var childIndex = parent.KeyRightChildIndex(initialSearch.KeyIndex);

            searchPath.Push(new SearchFrame(parent, childIndex));
            var current = parent.GetChild(childIndex);

            while (!current.IsLeaf)
            {
                searchPath.Push(new SearchFrame(current, TwoThreeChildIndex.Left));
                current = current.GetChild(TwoThreeChildIndex.Left);
            }

            //NOTE: successor key is always the left-most in found leaf node since that
            //is smallest
            return SearchContext.FoundKey(current, TwoThreeKeyIndex.Left, searchPath);
        }

        private void DeleteOnlyKeyFrom(TwoThreeNode<TKey, TValue> node, IStack<SearchFrame> searchPath, TwoThreeNode<TKey, TValue> attached)
        {
            Debug.Assert(node.KeyCount == 1, "Node should only have one key");

            //node to delete from only has one key
            //can't remove it since then leaves will not all be at the same level (unless this is the only leaf)
            //if any siblings have two keys then one can be 'borrowed' i.e moved to this node
            //if no siblings have any spare keys, the parent could have one to be borrowed
            //if neither the siblings nor the parent have any keys to borrow then the two remaining keys (the one in
            //the parent and the one in the only sibling) then merge these two keys into a new node and move the 'hole'
            //up to the parent and repeat the merging process
            if (searchPath.Count > 0)
            {
                var parentFrame = searchPath.Peek();
                var parentNode = parentFrame.Parent;
                var nodeChildIndex = parentFrame.ChildIndex;

                bool borrowed = parentNode.TryLendToChildFromSibling(nodeChildIndex, attached) || parentNode.TryLendToChild(nodeChildIndex, attached);
                if (!borrowed)
                {

                    var mergedNode = parentNode.MergeChild(nodeChildIndex);

                    searchPath.Pop();
                    DeleteOnlyKeyFrom(parentNode, searchPath, mergedNode);
                }
            }
            else
            {
                //no parents so the only key in the root node is being deleted
                Debug.Assert(node == this.root, "Should be at root with empty search path");
                this.root = attached;
            }
        }

        private void DeleteFromLeaf(SearchContext search)
        {
            var node = search.Parent;
            Debug.Assert(node.IsLeaf, "Must delete from leaf node");

            if (node.KeyCount == 2)
            {
                //since leaf is a 3-node, key can be removed without requiring any re-oragnisation
                if (search.KeyIndex == 0)
                {
                    node.BorrowLeftKey();
                }
                else
                {
                    node.BorrowRightKey();
                }
            }
            else
            {
                DeleteOnlyKeyFrom(node, search.SearchPath, null);
            }
        }

        /// <summary>Clears this tree.</summary>
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

        /// <summary>Gets an enumerator for the key-value pairs in this tree.</summary>
        /// <returns>An enumerator for the keys in this tree.</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return TwoThreeNode.IterateKeyPairsFrom(this.root).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        
        private void ApplyInsert(SearchContext searchContext, TKey key, TValue value)
        {
            var node = searchContext.Parent;

            //if search was successful then update value associated with the key at the relevant index
            if (searchContext.Found)
            {
                node.SetValueAt(searchContext.KeyIndex, value);
            }
            else
            {
                Debug.Assert(node == null || node.IsLeaf, "Failed search terminated at non-leaf node");
                var split = new TwoThreeNodeSplit<TKey, TValue>(null, null, new KeyValuePair<TKey, TValue>(key, value));

                Fixup(node, searchContext.SearchPath, searchContext.InsertChildIndex, split);

                this.count++;
            }
        }

        private void Fixup(TwoThreeNode<TKey, TValue> node, IStack<SearchFrame> searchPath, TwoThreeChildIndex childIndex, TwoThreeNodeSplit<TKey, TValue> split)
        {
            if (node == null)
            {
                Debug.Assert(searchPath.Count == 0, "Got null node with non-empty search path");
                this.root = TwoThreeNode<TKey, TValue>.Create2Node(split.MiddleKeyPair, leftChild: split.Left, right: split.Right);
            }
            else if (node.KeyCount == 1)
            {
                Debug.Assert(childIndex != TwoThreeChildIndex.Middle, "Unexpected key index for 2-node");

                node.FixupAt(childIndex, split);
            }
            else
            {
                Debug.Assert(node.KeyCount == 2, "Unexpected number of keys in node");

                //current node is already full so needs to be split
                //merge the previous split result into the current node to creat e a new split node
                var thisSplit = SplitNode(node, childIndex, split);                

                //if search path is empty then the root has been split
                if (searchPath.Count == 0)
                {
                    //handle root by making last recursive call with no parent
                    this.root = TwoThreeNode<TKey, TValue>.Create2Node(thisSplit.MiddleKeyPair, leftChild: thisSplit.Left, right: thisSplit.Right);
                }
                else
                {
                    var parentFrame = searchPath.Pop();
                    Fixup(parentFrame.Parent, searchPath, parentFrame.ChildIndex, thisSplit);
                }
            }
        }

        private static TwoThreeNodeSplit<TKey, TValue> SplitNode(TwoThreeNode<TKey, TValue> node, TwoThreeChildIndex childIndex, TwoThreeNodeSplit<TKey, TValue> previousSplit)
        {
            Debug.Assert(node.KeyCount == 2, "Node to split not full");

            if (childIndex == TwoThreeChildIndex.Left)
            {
                //split is being inserted into left subtree i.e. kvp.Key < keys[0]
                //split node into two 2-nodes
                //kvp.Key -> [p0, p1] and k1 -> [c1, c2]
                //k0 is the middle key of the three and will be propagated up to the parent node
                var leftNode = TwoThreeNode<TKey, TValue>.Create2Node(previousSplit.MiddleKeyPair, previousSplit.Left, previousSplit.Right);
                var rightNode = TwoThreeNode<TKey, TValue>.Create2Node(node.PairAt(TwoThreeKeyIndex.Right), node.GetChild(TwoThreeChildIndex.Middle), node.GetChild(TwoThreeChildIndex.Right));
                return new TwoThreeNodeSplit<TKey, TValue>(leftNode, rightNode, node.PairAt(TwoThreeKeyIndex.Left));
            }
            else if (childIndex == TwoThreeChildIndex.Middle)
            {
                //split is being inserted into middle subtree i.e. keys[0] < kvp.key < keys[1]
                //k0 -> [c0, p0] and k1 -> [p1, c2]
                //propagated key is the middle of the three and will be propagated again up to the parent
                var leftNode = TwoThreeNode<TKey, TValue>.Create2Node(node.PairAt(TwoThreeKeyIndex.Left), node.GetChild(TwoThreeChildIndex.Left), previousSplit.Left);
                var rightNode = TwoThreeNode<TKey, TValue>.Create2Node(node.PairAt(TwoThreeKeyIndex.Right), previousSplit.Right, node.GetChild(TwoThreeChildIndex.Right));
                return new TwoThreeNodeSplit<TKey, TValue>(leftNode, rightNode, previousSplit.MiddleKeyPair);
            }
            else
            {
                Debug.Assert(childIndex == TwoThreeChildIndex.Right, "Unexpected insert index");

                //split is being inserted into right subtree i.e. kvp.Key > keys[1]
                //split node into two 2-nodes
                //k0 -> [c0, c1], kvp.Key -> [p0, p1]
                //k1 is the middle key of the three and will be propagated up to the parent node
                var leftNode = TwoThreeNode<TKey, TValue>.Create2Node(node.PairAt(TwoThreeKeyIndex.Left), node.GetChild(TwoThreeChildIndex.Left), node.GetChild(TwoThreeChildIndex.Middle));
                var rightNode = TwoThreeNode<TKey, TValue>.Create2Node(previousSplit.MiddleKeyPair, previousSplit.Left, previousSplit.Right);
                return new TwoThreeNodeSplit<TKey, TValue>(leftNode, rightNode, node.PairAt(TwoThreeKeyIndex.Right));
            }
        }

        private SearchContext SearchFor(TKey key)
        {
            return SearchFor(this.root, key, this.comparer);
        }

        private class KeySearchResult
        {
            private TwoThreeKeyIndex keyIndex;
            private TwoThreeChildIndex childIndex;
            public bool Successful { get; private set; }

            public TwoThreeKeyIndex KeyIndex
            {
                get
                {
                    if (this.Successful) return this.keyIndex;
                    else throw new InvalidOperationException("No key index for failed search");
                }
            }

            public TwoThreeChildIndex ChildToSearch
            {
                get
                {
                    if (this.Successful) throw new InvalidOperationException("No child index for failed search");
                    else return this.childIndex;
                }
            }

            public static KeySearchResult Found(TwoThreeKeyIndex keyIndex)
            {
                return new KeySearchResult { Successful = true, keyIndex = keyIndex };
            }

            public static KeySearchResult Failed(TwoThreeChildIndex childIndex)
            {
                return new KeySearchResult { Successful = false, childIndex = childIndex };
            }
        }

        private static KeySearchResult TryFindKeyIndex(TwoThreeNode<TKey, TValue> node, TKey key, IComparer<TKey> keyComparer)
        {
            Debug.Assert(node != null);

            foreach (var i in node.KeyIndexes)
            {
                int c = keyComparer.Compare(key, node.KeyAt(i));
                if (c < 0)
                {
                    //key not found - return index of child to continue search
                    var childIndex = i == TwoThreeKeyIndex.Left ? TwoThreeChildIndex.Left : TwoThreeChildIndex.Middle;
                    return KeySearchResult.Failed(childIndex);
                }
                else if (c == 0)
                {
                    //key found - return index of matching key
                    return KeySearchResult.Found(i);
                }
            }

            //key is larger than all keys in the current node
            //bool isGreater = keyComparer.Compare(key, node.KeyAt(TwoThreeKeyIndex.Right)) > 0;
            //Debug.Assert(isGreater, "Key should be larger than all in node");

            //key not found - search should continue in right child
            return KeySearchResult.Failed(TwoThreeChildIndex.Right);
        }

        private static SearchContext SearchFor(TwoThreeNode<TKey, TValue> root, TKey key, IComparer<TKey> keyComparer)
        {
            if (root == null)
            {
                return SearchContext.NotFound(root, TwoThreeChildIndex.Left, new FixedStack<SearchFrame>(0));
            }

            TwoThreeNode<TKey, TValue> current = root;
            var searchPath = new DynamicStack<SearchFrame>();

            while (current != null)
            {
                var keySearch = TryFindKeyIndex(current, key, keyComparer);
                if (keySearch.Successful)
                {
                    //key matches in current node so return success
                    return SearchContext.FoundKey(current, keySearch.KeyIndex, searchPath);
                }
                else
                {
                    var childIndex = keySearch.ChildToSearch;
                    
                    //key does not exist in the current node. Returned index indicates the index of the child to continue
                    //searching. If this node is a leaf then the search has failed
                    if (current.IsLeaf) return SearchContext.NotFound(current, childIndex, searchPath);
                    else
                    {
                        //save this part of the search and search subtree
                        var child = current.GetChild(childIndex);
                        searchPath.Push(new SearchFrame(current, childIndex));
                        current = child;
                    }
                }
            }
            
            //should never happen!
            Debug.Fail("Continued search beyond a leaf node");
            throw new InvalidOperationException(":(");
        }

        private struct SearchFrame
        {
            public SearchFrame(TwoThreeNode<TKey, TValue> parent, TwoThreeChildIndex childIndex)
                : this()
            {
                this.Parent = parent;
                this.ChildIndex = childIndex;
            }

            public TwoThreeNode<TKey, TValue> Parent { get; private set; }
            public TwoThreeChildIndex ChildIndex { get; private set; }
        }

        private class SearchContext
        {
            private TwoThreeKeyIndex keyIndex;
            private TwoThreeChildIndex childIndex;

            public static SearchContext FoundKey(TwoThreeNode<TKey, TValue> node, TwoThreeKeyIndex keyIndex, IStack<SearchFrame> searchPath)
            {
                return new SearchContext { Found = true, Parent = node, keyIndex = keyIndex, SearchPath = searchPath };
            }

            public static SearchContext NotFound(TwoThreeNode<TKey, TValue> leaf, TwoThreeChildIndex insertChildIndex, IStack<SearchFrame> searchPath)
            {
                return new SearchContext { Found = false, Parent = leaf, childIndex = insertChildIndex, SearchPath = searchPath };
            }

            public bool Found { get; private set; }
            public TwoThreeNode<TKey, TValue> Parent { get; private set; }
            public TwoThreeKeyIndex KeyIndex
            {
                get
                {
                    if (this.Found) return this.keyIndex;
                    else throw new InvalidOperationException("No key index for failed search");
                }
            }

            public TwoThreeChildIndex InsertChildIndex
            {
                get
                {
                    if (this.Found) throw new InvalidOperationException("No insert child index for successful search");
                    else return this.childIndex;
                }
            }
            public IStack<SearchFrame> SearchPath { get; private set; }
        }
    }
}
