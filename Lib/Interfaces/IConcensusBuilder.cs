using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IConcensusBuilder
{
    public ICollection<Sequence> Concensus(ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences);
}