using System.Diagnostics.Contracts;
namespace NDS
{
    /// <summary>Represents a binary node in a tree.</summary>
    /// <typeparam name="TNode">Type of child nodes.</typeparam>
    public interface IBinaryNode<TNode> where TNode : IBinaryNode<TNode>
    {
        /// <summary>Gets/sets the left child.</summary>
        TNode Left { get; set; }

        /// <summary>Gets/sets the right child.</summary>
        TNode Right { get; set; }
    }

    public static class BinaryNodeExtensions
    {
        internal static TNode GetChild<TNode>(this IBinaryNode<TNode> node, BranchDirection direction)
            where TNode : IBinaryNode<TNode>
        {
            Contract.Requires(node != null);
            return (direction == BranchDirection.Left) ? node.Left : node.Right;
        }

        internal static void SetChild<TNode>(this IBinaryNode<TNode> parent, BranchDirection direction, TNode child)
            where TNode : IBinaryNode<TNode>
        {
            Contract.Requires(parent != null);

            if (direction == BranchDirection.Left)
            {
                parent.Left = child;
            }
            else
            {
                parent.Right = child;
            }
        }
    }
}
