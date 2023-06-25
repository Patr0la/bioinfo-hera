using Lib.Entities;

namespace Lib.Services;

public class SequenceVertexPath
{
    public SequenceEdge[] Path { get; init; } = null!;
    public SequenceVertex Current { get; init; } = null!;
}