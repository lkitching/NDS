using System;
using System.Collections.Generic;

namespace NDS.Graphs
{
    public interface IGraph<V>
    {
        IEnumerable<V> Vertices { get; }
        bool ContainsVertex(V vertex);
        IEnumerable<V> GetAdjacentVertices(V vertex);
        IEqualityComparer<V> VertexComparer { get; }
    }
}
