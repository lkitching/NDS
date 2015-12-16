using System;
using System.Collections.Generic;

namespace NDS.Graphs
{
    public interface IGraph<E, V>
    {
        IEnumerable<V> Vertices { get; }
        IEnumerable<E> GetAdjacentEdges(V vertex);
        bool ContainsVertex(V vertex);
        IEnumerable<V> GetAdjacentVertices(V vertex);
        IEqualityComparer<V> VertexComparer { get; }
        IEqualityComparer<E> EdgeComparer { get; }
    }
}
