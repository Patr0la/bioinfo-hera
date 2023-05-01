using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Lib.Entities;
using Lib.Interfaces;

namespace Cli.Commands;

[Command("generate", Description = "Generates random sequence.")]
public class GenerateRandom : ICommand
{
    private readonly IRandomSequenceGenerator _randomSequenceGenerator;

    public GenerateRandom(IRandomSequenceGenerator randomSequenceGenerator)
    {
        _randomSequenceGenerator = randomSequenceGenerator;
    }

    [CommandOption("length", 'l', Description = "Sequence length.")]
    public required int Length { get; init; }

    [CommandOption("repetitions", 'r', Description = "Force repetitions.")]
    public bool ForceRepetitions { get; init; } = false;

    [CommandOption("repetition-insert-chance", 'c', Description = "Chance of repetition insertion.")]
    public double repetitionInsertChance { get; init; } = 0.01;

    [CommandOption("min-repetition-length", 'm', Description = "Minimum repetition length.")]
    public int minRepetitionLength { get; init; }

    [CommandOption("max-repetition-length", 'x', Description = "Maximum repetition length.")]
    public int maxRepetitionLength { get; init; }


    public ValueTask ExecuteAsync(IConsole console)
    {
        var sequence = _randomSequenceGenerator.GenerateRandomSequence(
            SequenceType.DNA,
            Length,
            ForceRepetitions,
            repetitionInsertChance,
            minRepetitionLength: minRepetitionLength,
            maxRepetitionLength: maxRepetitionLength
        );

        console.Output.WriteLine(">Random sequence");
        console.Output.WriteLine(sequence.ToString());

        return default;
    }
}