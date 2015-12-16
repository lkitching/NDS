namespace NDS.Graphs
{
    /// <summary>Represents an edge between two vertices.</summary>
    /// <typeparam name="V">The vertex type.</typeparam>
    public interface IEdge<V>
    {
        V V1 { get; }
        V V2 { get; }
    }
}
