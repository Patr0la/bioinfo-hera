using Lib.Entities;
using Lib.Interfaces;
using MathNet.Numerics.Distributions;

namespace Lib.Services;

public class RandomSequenceGenerator : IRandomSequenceGenerator
{
    public Sequence GenerateRandomSequence(SequenceType type, int length, bool forceRepetitions = false,
        double repetitionInsertChance = 0D, int minRepetitionLength = 0,
        int maxRepetitionLength = 0)
    {
        var values = new int[length];

        var random = new Random();

        var i = 0;
        while (i < length)
        {
            values[i++] = random.Next(1, 4);

            if (!forceRepetitions) continue;

            var repetitions = random.Next(minRepetitionLength, maxRepetitionLength + 1);
            while (repetitions-- > 0 && i < length) values[i++] = values[i - 1];
        }

        return new Sequence(values);
    }

    public Sequence[] RandomlySplitSequence(Sequence sequence, int meanLength, int standardDeviation)
    {
        var sequences = new List<Sequence>();

        var normal = new Normal(meanLength, standardDeviation);

        var total = 0;
        while (total < sequence.Values.Length)
        {
            var length = (int)Math.Round(normal.Sample());
            if (length < 1) length = 1;

            if (length > sequence.Values.Length - total) length = sequence.Values.Length - total;

            var values = new int[length];
            Array.Copy(sequence.Values, total, values, 0, length);
            sequences.Add(new Sequence(values));
            total += length;
        }

        return sequences.ToArray();
    }
}