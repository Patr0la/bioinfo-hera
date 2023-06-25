using Lib.Entities;

namespace Lib.Interfaces;

public interface ISequenceBuilder
{
    public Sequence Build(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences);

    public ICollection<Sequence> DebugBuild(ICollection<Sequence> contigs, ICollection<Sequence> connections);

    public Sequence ConnectBetweenContigs(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences);
}