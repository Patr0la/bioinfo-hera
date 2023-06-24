using Lib.Entities;
using QuikGraph;
using System.Linq;
using QuikGraph.Algorithms.Search;

namespace Lib.Services;

public class GraphExtension
{
    public static ICollection<SequenceVertexScore>? ApproachOne(UndirectedGraph<SequenceVertex, SequenceEdge> graph)
    {
        var stack = new Stack<SequenceVertexPath>();
        var visited = new HashSet<string>();

        visited.Add("ctg2");

        var newEdges = graph.AdjacentEdges(new SequenceVertex()
            {
                Name = "ctg2"
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
            if (visited.Contains(v.Current.Name)) continue;

            if (v.Current.IsAnchor || graph.AdjacentVertices(v.Current).Any(v => v.IsAnchor && v.Name != "ctg2"))
            {
                var path = v.Path.ToList();
                
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

    public static void ExtendWithHighestOverlapScore(UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        SequenceVertex startingVertex)
    {
        // Color each vertex to keep track of visited nodes
        var vertexColorMap = new Dictionary<SequenceVertex, GraphColor>();
        var dfs = new UndirectedDepthFirstSearchAlgorithm<SequenceVertex, SequenceEdge>(
            null,
            graph,
            vertexColorMap,
            e => e.OrderByDescending(x => x.OverlapScore)
        );

        // Stack to track the DFS path
        var path = new Stack<SequenceVertex>();

        dfs.StartVertex += v => path.Push(v);
        dfs.FinishVertex += v => path.Pop();

        dfs.ExamineEdge += (o, e) =>
        {
            if (vertexColorMap[e.Target] == GraphColor.White)
            {
                path.Push(e.Target);
            }
            else if (path.Contains(e.Target)) // If we've reached a vertex already on the path, we have a cycle
            {
                throw new Exception("Cycle detected - aborting");
            }
        };

        dfs.BackEdge += (o, e) =>
        {
            if (path.Count > 0 && path.Peek() != e.Source)
            {
                path.Pop();
            }
        };

        dfs.TreeEdge += (o, e) =>
        {
            if (e.Target.IsAnchor && e.Target.Name != startingVertex.Name)
            {
                dfs.Abort();
            }
        };

        dfs.Compute(startingVertex);
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
