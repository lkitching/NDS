using NDS.Algorithms.Graphs;
using NUnit.Framework;

namespace NDS.Tests.Algorithms.Graphs
{
    [TestFixture]
    public class KruskalsAlgorithmTest : MinimumSpanningTreeAlgorithmTest
    {
        protected override IMinimumSpanningTreeAlgorithm Create()
        {
            return new KruskalsAlgorithm();
        }
    }
}
