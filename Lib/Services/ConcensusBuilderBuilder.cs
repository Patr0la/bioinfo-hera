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

    public ICollection<Sequence> Concensus(ICollection<ICollection<SequenceEdge>> paths,
        Dictionary<string, Sequence> sequences)
    {
        var concensus = new List<Sequence>();

        var builtPaths = paths.Select(path => _sequenceBuilder.ConnectBetweenContigs(path, sequences)).ToList();

        var minLength = builtPaths.Min(path => path.Data.Length);
        var maxLength = builtPaths.Max(path => path.Data.Length);

        if (maxLength - minLength > MAX_GROUP_SIZE)
        {
            var sortedGroupsBy1kb = builtPaths
                .GroupBy(path => (path.Data.Length - minLength) / 1000)
                .OrderBy(group => group.Key)
                .ToList();

            var newGroups = new List<ICollection<SequenceEdge>>();
            
            var averageFrequency = sortedGroupsBy1kb.Average(group => group.Count());

            /*
        var groups = builtPaths.GroupBy(path => path.Data.Length / MAX_GROUP_SIZE).ToList();

        foreach (var group in groups)
        {
            var lengthFrequencies = group.GroupBy(path => path.Data.Length).ToList();
            var mostFrequentGroup = lengthFrequencies.MaxBy(group => group.Count());
            var mostFrequentGroupFrequency = mostFrequentGroup.Count();
            
            var filteredLengthFrequency = lengthFrequencies
                .Where(group => group.Count() > mostFrequentGroupFrequency / 2)
                .ToList();
        }
                */
        }
        else
        {
            concensus.Add(new Sequence("group", string.Join("", builtPaths.Select(path => path.Data))));
        }

        return concensus;
    }
}