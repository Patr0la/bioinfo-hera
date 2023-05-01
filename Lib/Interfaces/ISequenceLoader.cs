using Lib.Entities;

namespace Lib.Interfaces;

public interface ISequenceLoader
{
    public Sequence[] LoadSequencesFromFastaStream(Stream stream);
}