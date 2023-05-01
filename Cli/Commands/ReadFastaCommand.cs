using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Lib.Interfaces;

namespace Cli.Commands;

[Command( "read",Description = "Reads a fasta file and prints the sequences.")]
public class ReadFastaCommand : ICommand
{
    private readonly ISequenceLoader _sequenceLoader;

    public ReadFastaCommand(ISequenceLoader sequenceLoader)
    {
        _sequenceLoader = sequenceLoader;
    }
    
    [CommandParameter(0, Description = "Path to the fasta file.")]
    public required string InputPath { get; init; }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var inputStream = File.OpenRead(InputPath);
        var sequences = _sequenceLoader.LoadSequencesFromFastaStream(inputStream);
        console.Output.WriteLine("Sequences:");
        foreach (var sequence in sequences)
        {
            console.Output.WriteLine(sequence.ToString());
        }
        
        return default;
    }
}