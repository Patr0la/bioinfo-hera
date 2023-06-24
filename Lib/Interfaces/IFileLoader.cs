using Lib.Entities;
using QuikGraph;

namespace Lib.Interfaces;

public interface IFileLoader
{
    public UndirectedGraph<SequenceVertex, SequenceEdge> LoadPaf(string overlapsRRPath, string overlapsCRPath);

    ICollection<Sequence> LoadFasta(string filePath);
}