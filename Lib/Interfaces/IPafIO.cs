using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IPafIO
{
    public BidirectionalGraph<SequenceVertex, SequenceEdge> LoadPaf(string overlapsRRPath, string overlapsCRPath, int MinContigOverlap, float MinSequenceIdentity);
}