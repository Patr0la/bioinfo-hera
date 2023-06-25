using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IGraphExtender
{
    public ICollection<SequenceEdge>? DFSByWeight(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Func<SequenceEdge, double> weightSelector
    );

    public ICollection<SequenceEdge>? MonteCarloSearch(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Random random,
        Func<SequenceEdge, double> weightSelector
    );
}