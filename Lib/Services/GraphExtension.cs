using Lib.Entities;
using QuikGraph;
using System.Linq;
using QuikGraph.Algorithms.Search;

namespace Lib.Services;

public class GraphExtension
{
    public static ICollection<SequenceVertexScore>? ApproachOne(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start
    )
    {
        var stack = new Stack<SequenceVertexPath>();
        var visited = new HashSet<string> { start };

        var newEdges = graph
            .AdjacentEdges(new SequenceVertex()
            {
                Name = start
            })
            .OrderBy(e => e.OverlapScore)
            .Select(e =>
                new SequenceVertexPath()
                {
                    Current = e.Target,
                    Path = new SequenceVertexScore[]
                    {
                        new SequenceVertexScore()
                        {
                            Name = e.Source.Name,
                            IsAnchor = e.Source.IsAnchor,
                            Score = e.OverlapScore
                        }
                    }
                }
            ).ToList();

        foreach (var e in newEdges)
        {
            stack.Push(e);
        }

        while (stack.Count > 0)
        {
            var v = stack.Pop();

            var ctgReached = graph.AdjacentVertices(v.Current).FirstOrDefault(v => v.IsAnchor && v.Name != start);
            if (v.Current.IsAnchor || ctgReached != null)
            {
                var path = v.Path.ToList();
                path.Add(new SequenceVertexScore()
                {
                    Name = v.Current.Name,
                    IsAnchor = v.Current.IsAnchor,
                    Score = 0
                });
                path.Add(new SequenceVertexScore()
                {
                    Name = ctgReached.Name,
                    IsAnchor = ctgReached.IsAnchor,
                    Score = 0
                });

                return path;
            }

            visited.Add(v.Current.Name);

            var newEdge = graph.AdjacentEdges(v.Current)
                .Where(e => !visited.Contains(e.Target.Name))
                .MaxBy(e => e.OverlapScore);

            if (newEdge == null)
                continue;

            stack.Push(v);

            var newPath = v.Path.ToList();
            newPath.Add(new SequenceVertexScore()
            {
                Name = newEdge.Source.Name,
                IsAnchor = newEdge.Source.IsAnchor,
                Score = newEdge.OverlapScore
            });
            stack.Push(new SequenceVertexPath()
            {
                Current = newEdge.Target,
                Path = newPath.ToArray(),
            });
        }

        return null;
    }

    public static ICollection<SequenceVertexScore>? ApproachTwo(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start
    )
    {
        var stack = new Stack<SequenceVertexPath>();
        var visited = new HashSet<string> { start };

        var newEdges = graph
            .AdjacentEdges(new SequenceVertex()
            {
                Name = start
            })
            .OrderBy(e => e.ExtensionScore)
            .Select(e =>
                new SequenceVertexPath()
                {
                    Current = e.Target,
                    Path = new SequenceVertexScore[]
                    {
                        new SequenceVertexScore()
                        {
                            Name = e.Source.Name,
                            IsAnchor = e.Source.IsAnchor,
                            Score = e.ExtensionScore
                        }
                    }
                }
            ).ToList();

        foreach (var e in newEdges)
        {
            stack.Push(e);
        }

        while (stack.Count > 0)
        {
            var v = stack.Pop();

            var ctgReached = graph.AdjacentVertices(v.Current).FirstOrDefault(v => v.IsAnchor && v.Name != start);
            if (v.Current.IsAnchor || ctgReached != null)
            {
                var path = v.Path.ToList();
                path.Add(new SequenceVertexScore()
                {
                    Name = v.Current.Name,
                    IsAnchor = v.Current.IsAnchor,
                    Score = 0
                });
                path.Add(new SequenceVertexScore()
                {
                    Name = ctgReached.Name,
                    IsAnchor = ctgReached.IsAnchor,
                    Score = 0
                });
                return path;
            }

            visited.Add(v.Current.Name);

            var newEdge = graph.AdjacentEdges(v.Current)
                .Where(e => !visited.Contains(e.Target.Name))
                .MaxBy(e => e.ExtensionScore);

            if (newEdge == null)
                continue;

            stack.Push(v);

            var newPath = v.Path.ToList();
            newPath.Add(new SequenceVertexScore()
            {
                Name = newEdge.Source.Name,
                IsAnchor = newEdge.Source.IsAnchor,
                Score = newEdge.ExtensionScore
            });
            stack.Push(new SequenceVertexPath()
            {
                Current = newEdge.Target,
                Path = newPath.ToArray(),
            });
        }

        return null;
    }

    public static ICollection<SequenceVertexScore>? ApproachTree(
        UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        string start,
        Random random
    )
    {
        var stack = new Stack<SequenceVertexPath>();
        var visited = new HashSet<string> { };

        stack.Push(new SequenceVertexPath()
        {
            Current = new SequenceVertex()
            {
                Name = start
            },
            Path = new SequenceVertexScore[] { }
        });

        while (stack.Count > 0)
        {
            var v = stack.Pop();

            var ctgReached = graph.AdjacentVertices(v.Current).FirstOrDefault(v => v.IsAnchor && v.Name != start);
            if ((v.Current.IsAnchor && v.Current.Name != start) || ctgReached != null)
            {
                var path = v.Path.ToList();
                path.Add(new SequenceVertexScore()
                {
                    Name = v.Current.Name,
                    IsAnchor = v.Current.IsAnchor,
                    Score = 0
                });
                path.Add(new SequenceVertexScore()
                {
                    Name = ctgReached.Name,
                    IsAnchor = ctgReached.IsAnchor,
                    Score = 0
                });
                return path;
            }

            visited.Add(v.Current.Name);

            var newEdge = WeightedPick(
                graph.AdjacentEdges(v.Current)
                    .Where(v => !visited.Contains(v.Target.Name))
                    .ToList()
                , random);

            if (newEdge == null)
                continue;

            stack.Push(v);

            var newPath = v.Path.ToList();
            newPath.Add(new SequenceVertexScore()
            {
                Name = newEdge.Source.Name,
                IsAnchor = newEdge.Source.IsAnchor,
                Score = newEdge.ExtensionScore
            });
            stack.Push(new SequenceVertexPath()
            {
                Current = newEdge.Target,
                Path = newPath.ToArray(),
            });
        }

        return null;
    }

    public static SequenceEdge? WeightedPick(ICollection<SequenceEdge> vertices, Random random)
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
    public SequenceVertexScore[] Path { get; set; }
    public SequenceVertex Current { get; set; }
}

public class SequenceVertexScore : SequenceVertex
{
    public double Score { get; set; }
}