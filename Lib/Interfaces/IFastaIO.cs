using Lib.Entities;

namespace Lib.Interfaces;

public interface IFastaIO
{
    public Dictionary<string, Sequence> LoadFasta(string filePath);
    public void SaveFasta(string filePath, ICollection<Sequence> sequences);
}