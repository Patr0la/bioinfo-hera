using Lib.Entities;

namespace Lib.Interfaces;

public interface ISequenceBuilder
{
    public Sequence Build(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences);

    public ICollection<Sequence> DebugBuild(Sequence ctg1, ICollection<SequenceEdge> ctg1ctg2, Sequence ctg2,
        ICollection<SequenceEdge> ctg2ctg3, Sequence ctg3, Dictionary<string, Sequence> sequences);

    public Sequence ConnectBetweenContigs(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences);
}