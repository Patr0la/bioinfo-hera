using Lib.Entities;
using QuikGraph;

namespace Lib.Services;

public static class GraphExtension
{
    public static ICollection<SequenceEdge>? ApproachOne(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Func<SequenceEdge, double> weightSelector
    )
    {
        var stack = new Stack<SequenceVertexPath>();
        var visited = new HashSet<string> { start };

        var newEdges = graph
            .AdjacentEdges(new SequenceVertex()
            {
                Name = start
            })
            .OrderBy(weightSelector)
            .Select(e =>
                new SequenceVertexPath()
                {
                    Current = e.Target,
                    Path = new[] { e }
                }
            ).ToList();

        foreach (var e in newEdges)
        {
            stack.Push(e);
        }

        while (stack.Count > 0)
        {
            var v = stack.Pop();

            var ctgReachedEdge = graph.AdjacentEdges(v.Current)
                .FirstOrDefault(e =>
                    (e.Target.IsAnchor || e.Source.IsAnchor) && (e.Source.Name != start && e.Target.Name != start));
            if (ctgReachedEdge != null)
            {
                var path = v.Path.ToList();
                path.Add(ctgReachedEdge);
                return path;
            }

            visited.Add(v.Current.Name);

            var newEdge = graph.AdjacentEdges(v.Current)
                .Where(e => !visited.Contains(e.Target.Name))
                .MaxBy(weightSelector);

            if (newEdge == null)
                continue;

            stack.Push(v);

            var newPath = v.Path.ToList();
            newPath.Add(newEdge);

            stack.Push(new SequenceVertexPath()
            {
                Current = newEdge.Target,
                Path = newPath.ToArray(),
            });
        }

        return null;
    }

    public static ICollection<SequenceEdge>? ApproachTree(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Random random
    )
    {
        var stack = new Stack<SequenceVertexPath>();
        var visited = new HashSet<string>();

        stack.Push(new SequenceVertexPath()
        {
            Current = new SequenceVertex()
            {
                Name = start
            },
            Path = new SequenceEdge[] { }
        });

        while (stack.Count > 0)
        {
            var v = stack.Pop();

            var ctgReachedEdge = graph.AdjacentEdges(v.Current)
                .FirstOrDefault(e =>
                    (e.Target.IsAnchor || e.Source.IsAnchor) && (e.Source.Name != start && e.Target.Name != start));
            if (ctgReachedEdge != null)
            {
                var path = v.Path.ToList();
                path.Add(ctgReachedEdge);
                return path;
            }

            visited.Add(v.Current.Name);

            var newEdge = WeightedPick(
                graph.AdjacentEdges(v.Current)
                    .Where(ae => !visited.Contains(ae.Target.Name))
                    .ToList()
                , random);

            if (newEdge == null)
                continue;

            stack.Push(v);

            var newPath = v.Path.ToList();

            newPath.Add(newEdge);

            stack.Push(new SequenceVertexPath()
            {
                Current = newEdge.Target,
                Path = newPath.ToArray(),
            });
        }

        return null;
    }

    private static SequenceEdge? WeightedPick(ICollection<SequenceEdge> vertices, Random random)
    {
        var sum = vertices.Sum(v => v.ExtensionScore);

        if (sum == 0)
            return null;

        var pick = random.NextDouble() * sum;

        foreach (var v in vertices)
        {
            pick -= v.ExtensionScore;
            if (pick <= 0)
                return v;
        }

        return vertices.LastOrDefault();
    }
}

class SequenceVertexPath
{
    public SequenceEdge[] Path { get; init; } = null!;
    public SequenceVertex Current { get; init; } = null!;
}