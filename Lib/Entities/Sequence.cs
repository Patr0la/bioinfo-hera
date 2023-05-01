using System.Text;

namespace Lib.Entities;

public class Sequence
{
    public static int ERR = 0;
    public static int A = 1;
    public static int T = 2;
    public static int G = 3;
    public static int C = 4;


    public int[] Values { get; set; }
    public SequenceType Type { get; set; }
    public string Name { get; set; }

    public Sequence(int[] values, SequenceType type = SequenceType.DNA, string name = "")
    {
        Values = values;
        Type = type;
        Name = name;
    }


    private const int PRINT_WIDTH = 80;

    private readonly Dictionary<int, char> _valueToChar = new()
    {
        { ERR, '-' },
        { A, 'A' },
        { T, 'T' },
        { G, 'G' },
        { C, 'C' }
    };

    public override string ToString()
    {
        var sb = new StringBuilder();

        var i = 0;
        while (i < Values.Length)
        {
            var j = 0;
            while (j < PRINT_WIDTH && i < Values.Length)
            {
                sb.Append(_valueToChar.GetValueOrDefault(Values[i]));
                i++;
                j++;
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }
}