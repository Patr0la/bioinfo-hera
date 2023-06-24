using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IPafIO
{
    public UndirectedGraph<SequenceVertex, SequenceEdge> LoadPaf(string overlapsRRPath, string overlapsCRPath);
}