using System;
using System.Diagnostics.Contracts;

namespace NDS
{
    /// <summary>Represents an item with a parent.</summary>
    /// <typeparam name="T">The type of the parent.</typeparam>
    public interface IHasParent<T>
    {
        /// <summary>Gets and sets the parent.</summary>
        T Parent { get; set;  }
    }

    internal static class HasParentExtensions
    {
        /// <summary>Gets the direction the given node is from its parent.</summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="node">The node.</param>
        /// <returns>Left if <paramref name="node"/> is the left child of its parent, otherwise Right</returns>
        internal static BranchDirection GetDirectionFromParent<TNode>(this TNode node)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            Contract.Requires(node != null);
            Contract.Requires(node.Parent != null);

            var parent = node.Parent;
            return Object.ReferenceEquals(node, parent.Left) ? BranchDirection.Left : BranchDirection.Right;
        }

        /// <summary>Gets the sibling node of the given node.</summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="node">The node whose sibling to find.</param>
        /// <returns>The sibling node of <paramref name="node"/>.</returns>
        public static TNode GetSibling<TNode>(this TNode node)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            Contract.Requires(node != null);
            Contract.Requires(node.Parent != null);

            var siblingDir = node.GetDirectionFromParent().OppositeDirection();
            return node.Parent.GetChild(siblingDir);
        }

        /// <summary>Gets the uncle\aunt node of this node i.e. its parent's sibling node.</summary>
        /// <typeparam name="TNode">The type of node in the tree.</typeparam>
        /// <param name="node">The node whose uncle to find.</param>
        /// <returns>The uncle node of this node.</returns>
        public static TNode GetUncle<TNode>(this TNode node)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            Contract.Requires(node != null);
            Contract.Requires(node.Parent != null);
            Contract.Requires(node.Parent.Parent != null);

            return node.Parent.GetSibling();
        }

        /// <summary>
        /// Rotates the given node above its parent. The direction of the rotation
        /// depends on which child <paramref name="node"/> is of the parent node.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="node">The node to rotate above it's parent.</param>
        public static void RotateAboveParent<TNode>(this TNode node)
            where TNode : class, IBinaryNode<TNode>, IHasParent<TNode>
        {
            var dir = node.GetDirectionFromParent();
            if (dir == BranchDirection.Left)
            {
                node.RotateRight();
            }
            else
            {
                node.RotateLeft();
            }
        }

        /// <summary>Finds the root node in the tree containing <paramref name="node"/>./</summary>
        /// <typeparam name="TNode">The type of nodes in the tree.</typeparam>
        /// <param name="node">A node in the tree.</param>
        /// <returns>The root node in the tree containing <paramref name="node"/> or null if <paramref name="node"/> is null.</returns>
        public static TNode FindRoot<TNode>(this TNode node)
            where TNode : class, IHasParent<TNode>
        {
            if (node == null) return null;

            var current = node;
            while (current.Parent != null)
            {
                current = current.Parent;
            }

            return current;
        }
    }
}
