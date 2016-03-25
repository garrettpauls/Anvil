using System.Collections.Generic;

using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Search;

namespace Anvil.Framework.QuickGraph.Algorithms
{
    public sealed class TopologicalSortSupportingCyclesAlgorithm<TVertex, TEdge> : AlgorithmBase<IVertexListGraph<TVertex, TEdge>>
        where TEdge : IEdge<TVertex>
    {
        public TopologicalSortSupportingCyclesAlgorithm(IVertexListGraph<TVertex, TEdge> graph)
            : base(graph)
        {
            SortedVertices = new List<TVertex>(graph.VertexCount);
        }

        public IList<TVertex> SortedVertices { get; }

        private void _FinishVertex(TVertex vertex)
        {
            SortedVertices.Insert(0, vertex);
        }

        protected override void InternalCompute()
        {
            var depthFirstSearch = new DepthFirstSearchAlgorithm<TVertex, TEdge>(VisitedGraph);
            try
            {
                depthFirstSearch.FinishVertex += _FinishVertex;
                depthFirstSearch.Compute();
            }
            finally
            {
                depthFirstSearch.FinishVertex -= _FinishVertex;
            }
        }
    }

    public static class TopologicalSortSupportingCyclesAlgorithmExtensions
    {
        public static IEnumerable<TVertex> TopologicalSortSupportingCycles<TVertex, TEdge>(this IVertexListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var sortAlgorithm = new TopologicalSortSupportingCyclesAlgorithm<TVertex, TEdge>(graph);
            sortAlgorithm.Compute();
            return sortAlgorithm.SortedVertices;
        }
    }
}
