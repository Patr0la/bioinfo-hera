using System.Text;
using Lib.Entities;
using Lib.Interfaces;

namespace Lib.Services;

public class FastaIO : IFastaIO
{
    public Dictionary<string, Sequence> LoadFasta(string filePath)
    {
        var sequences = new Dictionary<string, Sequence>();

        string? line;
        var sequence = new StringBuilder();
        var name = "";
        using var reader = new StreamReader(filePath);
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith(">"))
            {
                if (sequence.Length > 0)
                {
                    sequences.Add(name, new Sequence(name, sequence.ToString()));
                }

                name = line.Substring(1);
                sequence.Clear();
            }
            else
            {
                sequence.Append(line);
            }
        }

        if (sequence.Length > 0)
        {
            sequences.Add(name, new Sequence(name, sequence.ToString()));
        }

        return sequences;
    }

    
    public void SaveFasta(string filePath, ICollection<Sequence> sequences)
    {
        var sb = new StringBuilder();
        
        foreach (var sequence in sequences)
        {
            sb.AppendLine($">{sequence.Name}");
            sb.AppendLine(sequence.Data);
        }
        
        File.WriteAllText(filePath, sb.ToString());
    }
}