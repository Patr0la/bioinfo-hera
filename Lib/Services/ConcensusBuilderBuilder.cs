using System.Text;
using Lib.Entities;
using Lib.Interfaces;

namespace Lib.Services;

public class ConsensusBuilder : IConsensusBuilder
{
    private readonly ISequenceBuilder _sequenceBuilder;

    public ConsensusBuilder(ISequenceBuilder sequenceBuilder)
    {
        _sequenceBuilder = sequenceBuilder;
    }

    public Sequence Concensus(
        ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences,
        int GroupSizeMinDifference,
        int GroupSizeWindow)
    {
        var builtPaths = paths.Select(path => _sequenceBuilder.ConnectBetweenContigs(path, sequences)).ToList();

        var minLength = builtPaths.Min(path => path.Data.Length);
        var maxLength = builtPaths.Max(path => path.Data.Length);

        if (maxLength - minLength > GroupSizeMinDifference)
        {
            var sortedGroupsBy1kb = builtPaths
                .GroupBy(path => (path.Data.Length - minLength) / GroupSizeWindow)
                .OrderBy(group => group.Key)
                .ToList();

            var bestGroup = sortedGroupsBy1kb.MaxBy(group => group.Count());

            var mostCommonSameLengthSequences = bestGroup.GroupBy(x => x.Data.Length)
                .MaxBy(g => g.Count())
                .ToList();

            return mostCommonSameLengthSequences.GroupBy(x => x.Data)
                .MaxBy(g => g.Count())
                .First();
        }

        return new Sequence("resultV2", builtPaths.First().Data);
    }
}