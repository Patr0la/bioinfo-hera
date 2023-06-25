using Lib.Entities;

namespace Lib.Interfaces;

public interface IConsensusBuilder
{
    public Sequence Concensus(
        ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences,
        int GroupSizeMinDifference,
        int GroupSizeWindow);
}