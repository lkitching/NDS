using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>The direction of a branch taken in a search through a binary search tree.</summary>
    internal enum BranchDirection { Left, Right }

    /// <summary>Represents a branch taken in a search through a binary search tree.</summary>
    /// <typeparam name="T">The type of nodes in the tree.</typeparam>
    internal struct SearchBranch<T> : IEquatable<SearchBranch<T>>
    {
        public SearchBranch(T node, BranchDirection direction) : this()
        {
            Contract.Requires(node != null);

            this.Node = node;
            this.Direction = direction;
        }

        /// <summary>Gets the node the branch was taken at.</summary>
        public T Node { get; private set; }

        /// <summary>The direction of the branch i.e. which subtree of Node the search continued in.</summary>
        public BranchDirection Direction { get; set; }

        public bool Equals(SearchBranch<T> other)
        {
            return Object.Equals(this.Node, other.Node) && this.Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            return obj is SearchBranch<T> && (this.Equals((SearchBranch<T>)obj));
        }

        public override int GetHashCode()
        {
            return this.Node.GetHashCode() ^ this.Direction.GetHashCode();
        }
    }

    /// <summary>Represents a search for a key in binary search tree.</summary>
    /// <typeparam name="T">The type of nodes in the tree.</typeparam>
    internal interface IBSTSearchContext<T>
    {
        /// <summary>Gets the search path taken through the tree.</summary>
        ArrayList<SearchBranch<T>> SearchPath { get; }

        /// <summary>Whether the search was successful.</summary>
        bool Found { get; }

        /// <summary>
        /// The matching node if the search was successful. Null if the
        /// key was not found.
        /// </summary>
        T MatchingNode { get; }
    }

    internal class BSTSearch
    {
        /// <summary>Searches for a key in a binary search tree with the given root node.</summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <typeparam name="TKey">Key type of the tree.</typeparam>
        /// <typeparam name="TValue">Value type of the tree.</typeparam>
        /// <param name="root">The root node of the tree or null if the tree is empty.</param>
        /// <param name="key">The key to search for.</param>
        /// <param name="keyComparer">Comparer for keys.</param>
        /// <returns>A context representing the path taken for the search.</returns>
        internal static IBSTSearchContext<TNode> SearchFor<TNode, TKey, TValue>(TNode root, TKey key, IComparer<TKey> keyComparer)
            where TNode : class, IBSTNode<TNode, TKey, TValue>
        {
            Contract.Requires(keyComparer != null);

            var current = root;
            var searchPath = new ArrayList<SearchBranch<TNode>>(20);

            while (current != null)
            {
                int c = keyComparer.Compare(key, current.Key);
                if (c == 0)
                {
                    //key found
                    return new BSTSearchContext<TNode>
                    {
                        SearchPath = searchPath,
                        Found = true,
                        MatchingNode = current
                    };
                }
                else if (c < 0)
                {
                    //search left subtree
                    searchPath.Add(new SearchBranch<TNode>(current, BranchDirection.Left));
                    current = current.Left;
                }
                else
                {
                    //search right subtree
                    searchPath.Add(new SearchBranch<TNode>(current, BranchDirection.Right));
                    current = current.Right;
                }
            }

            //not found
            return new BSTSearchContext<TNode>
            {
                SearchPath = searchPath,
                Found = false,
                MatchingNode = null
            };
        }

        private class BSTSearchContext<T> : IBSTSearchContext<T>
        {
            private T matchingNode;

            public ArrayList<SearchBranch<T>> SearchPath { get; set; }
            public bool Found { get; set; }
            public T MatchingNode
            {
                get
                {
                    if (Found) return this.matchingNode;
                    else throw new InvalidOperationException("No matching node for unsuccessful search");
                }
                set { this.matchingNode = value; }
            }
        }
    }
}
