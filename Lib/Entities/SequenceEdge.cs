using Lib.Interfaces;
using QuikGraph;

namespace Lib.Entities;

public class SequenceEdge : IEdge<SequenceVertex>
{
    public SequenceVertex Source { get; set; }
    public SequenceVertex Target { get; set; }

    public bool SameStrand { get; set; }

    public int SourceStart { get; set; }
    public int SourceEnd { get; set; }
    public int SourceEndLenght => SourceEnd - SourceStart;

    public int TargetStart { get; set; }
    public int TargetEnd { get; set; }
    public int TargetEndLenght => TargetEnd - TargetStart;


    public float OverlapScore { get; set; }
    public float ExtensionScore { get; set; }
    public float SequenceIdentity { get; set; }
}