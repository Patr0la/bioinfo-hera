using System.Text;

namespace Lib.Entities;

public class Sequence
{
    public string Name { get; set; }
    public string Data { get; set; }

    public Sequence(string name, string data)
    {
        Name = name;
        Data = data;
    }
}