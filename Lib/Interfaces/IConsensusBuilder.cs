using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IConsensusBuilder
{
    public Sequence Concensus(ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences);
}