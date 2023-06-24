using Lib.Interfaces;
using QuikGraph;

namespace Lib.Entities;

public class SequenceEdge : IEdge<SequenceVertex>
{
    public SequenceVertex Source { get; set; }
    public SequenceVertex Target { get; set; }
    
    public bool SameStrand { get; set; }
    
    
    
    public float OverlapScore { get; set; }
    public float ExtensionScore { get; set; }
    public float SequenceIdentity { get; set; }
}