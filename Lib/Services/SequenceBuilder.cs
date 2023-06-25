using System.Text;
using Lib.Entities;

namespace Lib.Interfaces;

public class SequenceBuilder : ISequenceBuilder
{
    private readonly IPafIO _pafIo;

    public SequenceBuilder(IPafIO pafIo)
    {
        _pafIo = pafIo;
    }

    public Sequence Build(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences)
    {
        var sb = new StringBuilder();
        int index = 0;

        sb.Append(sequences[path.First().Source.Name].Data, 0, path.First().SourceEnd);

        foreach (var overlap in path)
        {
            var targetSeq = sequences[overlap.Target.Name];

            sb.Append(targetSeq.Data, overlap.TargetEnd, targetSeq.Data.Length - overlap.TargetEnd);
        }

        return new Sequence("result", sb.ToString());
    }

    public ICollection<Sequence> DebugBuild(ICollection<Sequence> contigs, ICollection<Sequence> connections)
    {
        var res = new List<Sequence>();
        var i = 0;
        foreach (var contig in contigs)
        {
            res.Add(contig);
            if (i < connections.Count)
            {
                res.Add(connections.ElementAt(i));
            }

            i++;
        }

        return res;
    }

    public Sequence ConnectBetweenContigs(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences)
    {
        var sb = new StringBuilder(200000);
        int index = 0;

        foreach (var overlap in path)
        {
            var targetSeq = sequences[overlap.Target.Name];

            sb.Append(targetSeq.Data, overlap.TargetEnd, targetSeq.Data.Length - overlap.TargetEnd);
        }

        var last = path.Last();
        sb.Remove(sb.Length - (last.TargetEnd - last.TargetStart), last.TargetEnd - last.TargetStart);

        return new Sequence("result", sb.ToString());
    }
}