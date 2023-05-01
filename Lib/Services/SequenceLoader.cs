using Lib.Entities;
using Lib.Interfaces;

namespace Lib.Services;

public class SequenceLoader : ISequenceLoader
{
    private readonly IRandomSequenceGenerator _randomSequenceGenerator;

    private readonly Dictionary<char, int> _valueMap = new()
    {
        { 'A', Sequence.A },
        { 'T', Sequence.T },
        { 'G', Sequence.G },
        { 'C', Sequence.C },
        { '-', Sequence.ERR }
    };

    public SequenceLoader(IRandomSequenceGenerator randomSequenceGenerator)
    {
        _randomSequenceGenerator = randomSequenceGenerator;
    }

    public Sequence[] LoadSequencesFromFastaStream(Stream stream)
    {
        List<Sequence> sequences = new();

        using var reader = new StreamReader(stream);

        var name = "";
        var data = new List<int>();
        var readingData = false;

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrEmpty(line)) continue;
            
            line = line.Trim();
            var lineType = line[0];
            
            if (lineType == ';') continue;
            
            if (lineType == '>')
            {
                if (readingData)
                {
                    sequences.Add(new Sequence(data.ToArray(), name: name));
                    data.Clear();
                }

                name = line.Substring(1);
                readingData = true;
                
                continue;
            }

            var lineData = line.ToCharArray();

            data.AddRange(lineData.Select(c => _valueMap.GetValueOrDefault(c, Sequence.ERR)));
        }

        if (readingData)
        {
            sequences.Add(new Sequence(data.ToArray(), name: name));
        }

        return sequences.ToArray();
    }
}