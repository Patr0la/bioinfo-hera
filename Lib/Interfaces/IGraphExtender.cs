using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IGraphExtender
{
    public ICollection<ICollection<SequenceEdge>?> DFSByWeight(BidirectionalGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Func<SequenceEdge, double> weightSelector);

    public ICollection<SequenceEdge>? MonteCarloSearch(
        BidirectionalGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Random random,
        Func<SequenceEdge, double> weightSelector
    );
}