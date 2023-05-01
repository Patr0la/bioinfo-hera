using Lib.Entities;

namespace Tests.Lib.RandomSequenceGeneratorTests;

public class GenerateRandomSequenceTests
{
    private readonly RandomSequenceGenerator _randomSequenceGenerator = new();

    [Theory]
    [InlineData(10, false, 0.01, 10, 100)]
    [InlineData(1000, true, 0.01, 10, 100)]
    [InlineData(100000, false, 0.01, 0, 0)]
    public void GenerateRandomSequence_WhenCalled_Returns_SequenceWithCorrectLength(
        int length,
        bool forceRepetitions,
        float repetitionInsertChance,
        int minRepetitionLength,
        int maxRepetitionLength)
    {
        // Act
        var sequence = _randomSequenceGenerator.GenerateRandomSequence(
            SequenceType.DNA,
            length,
            forceRepetitions,
            repetitionInsertChance,
            minRepetitionLength: minRepetitionLength,
            maxRepetitionLength: maxRepetitionLength
        );

        // Assert
        Assert.Equal(length, sequence.Values.Length);
    }

    [Theory]
    [InlineData(100, 0.1, 10, 100)]
    [InlineData(1000, 0.1, 10, 100)]
    [InlineData(10000, 0.01, 100, 1000)]
    public void GenerateRandomSequence_WhenCalled_Returns_SequenceWithRepeats(
        int length,
        float repetitionInsertChance,
        int minRepetitionLength,
        int maxRepetitionLength)
    {
        // Act
        var sequence = _randomSequenceGenerator.GenerateRandomSequence(
            SequenceType.DNA,
            length,
            true,
            repetitionInsertChance,
            minRepetitionLength: minRepetitionLength,
            maxRepetitionLength: maxRepetitionLength
        );

        // Assert
        Assert.True(sequence.Values.Length == length);

        var repetitions = 0;
        var i = 0;
        while (i < sequence.Values.Length)
        {
            var value = sequence.Values[i];
            var foundRepetition = true;
            for (var j = i + 1; j < sequence.Values.Length && j < i + maxRepetitionLength; j++)
                if (sequence.Values[j] != value)
                {
                    foundRepetition = false;
                    break;
                }

            if (foundRepetition)
            {
                repetitions++;
                i += maxRepetitionLength;
            }
            else
            {
                i++;
            }
        }

        Assert.True(repetitions > 0);
    }
}