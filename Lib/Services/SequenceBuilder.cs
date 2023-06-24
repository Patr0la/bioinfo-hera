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

    public ICollection<Sequence> DebugBuild(Sequence ctg1, ICollection<SequenceEdge> ctg1ctg2, Sequence ctg2, ICollection<SequenceEdge> ctg2ctg3, Sequence ctg3,
        Dictionary<string, Sequence> sequences)
    {
        return new List<Sequence>()
        {
            ctg1,
            ConnectBetweenContigs(ctg1ctg2, sequences),
            ctg2,
            ConnectBetweenContigs(ctg2ctg3, sequences),
            ctg3,
        };
    }
    
    public Sequence ConnectBetweenContigs(ICollection<SequenceEdge> path, Dictionary<string, Sequence> sequences)
    {
        var sb = new StringBuilder();
        int index = 0;
        
        foreach (var overlap in path)
        {
            var targetSeq = sequences[overlap.Target.Name];

            sb.Append(targetSeq.Data, overlap.TargetEnd, targetSeq.Data.Length - overlap.TargetEnd);
        }

        return new Sequence("result", sb.ToString());
    }
}