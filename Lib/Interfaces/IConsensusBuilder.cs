using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IConsensusBuilder
{
    public ICollection<Sequence> Concensus(ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences);
}