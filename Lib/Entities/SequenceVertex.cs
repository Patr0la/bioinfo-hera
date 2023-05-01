namespace Lib.Entities;

public class SequenceVertex : IEquatable<SequenceVertex>
{
    public string Name { get; set; }
    public bool IsAnchor { get; set; }

    public bool Equals(SequenceVertex? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SequenceVertex)obj);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}