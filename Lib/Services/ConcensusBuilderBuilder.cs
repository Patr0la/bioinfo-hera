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

    private const int MAX_GROUP_SIZE = 10000;

    public Sequence Concensus(ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences)
    {
        var builtPaths = paths.Select(path => _sequenceBuilder.ConnectBetweenContigs(path, sequences)).ToList();

        var minLength = builtPaths.Min(path => path.Data.Length);
        var maxLength = builtPaths.Max(path => path.Data.Length);

        if (maxLength - minLength > MAX_GROUP_SIZE)
        {
            var sortedGroupsBy1kb = builtPaths
                .GroupBy(path => (path.Data.Length - minLength) / 1000)
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