using Lib.Entities;

namespace Lib.Interfaces;

public interface IRandomSequenceGenerator
{
    public Sequence GenerateRandomSequence(
        SequenceType type,
        int length,
        bool forceRepetitions = false,
        double repetitionInsertChance = 0,
        int minRepetitionLength = 0,
        int maxRepetitionLength = 0
    );

    public Sequence[] RandomlySplitSequence(
        Sequence sequence,
        int meanLength,
        int standardDeviation
    );
}