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
}
