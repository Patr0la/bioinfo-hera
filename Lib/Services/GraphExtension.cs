using System.Collections.Concurrent;
using Lib.Entities;
using Lib.Interfaces;
using QuikGraph;

namespace Lib.Services;

public class GraphExtender : IGraphExtender
{
    public ICollection<ICollection<SequenceEdge>?> DFSByWeight(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Func<SequenceEdge, double> weightSelector
    )
    {
        var foundPaths = new List<ICollection<SequenceEdge>?>();

        var startableEdges = graph.AdjacentEdges(new SequenceVertex()
        {
            Name = start
        }).ToList();

        var newEdges = startableEdges
            .OrderByDescending(weightSelector)
            .Take((int)(startableEdges.Count * 0.2))
            .Select(e =>
                new SequenceVertexPath()
                {
                    Current = e.Target,
                    Path = new[] { e }
                }
            ).ToList();

        Parallel.ForEach(Partitioner.Create(0, newEdges.Count), range =>
        {
            for (var i = range.Item1; i < range.Item2; i++)
            {
                var startingEdge = newEdges[i];

                var stack = new Stack<SequenceVertexPath>();
                var visited = new HashSet<string> { start };

                stack.Push(startingEdge);

                while (stack.Count > 0)
                {
                    var v = stack.Pop();

                    var ctgReachedEdge = graph.AdjacentEdges(v.Current)
                        .FirstOrDefault(e =>
                            (e.Target.IsAnchor || e.Source.IsAnchor) &&
                            (e.Source.Name != start && e.Target.Name != start));
                    if (ctgReachedEdge != null)
                    {
                        var path = v.Path.ToList();
                        path.Add(ctgReachedEdge);

                        foundPaths.Add(path);
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
            }
        });


        return foundPaths;
    }

    public ICollection<SequenceEdge>? MonteCarloSearch(UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Random random, Func<SequenceEdge, double> weightSelector)
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
                    .ToList(),
                random,
                weightSelector);

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

    private static SequenceEdge? WeightedPick(ICollection<SequenceEdge> vertices, Random random,
        Func<SequenceEdge, double> weightSelector)
    {
        var sum = vertices.Sum(weightSelector);

        if (sum == 0)
            return null;

        var pick = random.NextDouble() * sum;

        foreach (var v in vertices)
        {
            pick -= weightSelector(v);
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