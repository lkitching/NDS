using NDS.Graphs;
using System.Collections.Generic;

namespace NDS.Algorithms.Graphs
{
    public interface IMinimumSpanningTreeAlgorithm
    {
        IEnumerable<E> MinimumSpanningTree<E, V, W>(IGraph<E, V> graph, IComparer<W> weightComparer)
            where E : IEdge<V>, IHasWeight<W>;
    }
}
