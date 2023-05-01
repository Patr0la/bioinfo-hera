using Lib.Entities;
using QuikGraph;
using System.Linq;

namespace Lib.Services;

public class GraphExtension
{
    public static ICollection<SequenceVertex>? ApproachOne(UndirectedGraph<SequenceVertex, SequenceEdge> graph)
    {
        var que = new Stack<SequenceVertexPath>();
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
                    Path = new SequenceVertex[] { e.Source }
                }
            ).ToList();

        foreach (var e in newEdges)
        {
            que.Push(e);
        }
        
        while (que.Count > 0)
        {
            var v = que.Pop();
            if (visited.Contains(v.Current.Name)) continue;

            if (v.Current.IsAnchor)
            {
                var path = v.Path.ToList();
                path.Add(v.Current);

                return path;
            } 

            visited.Add(v.Current.Name);

            var newEdges2 = graph.AdjacentEdges(v.Current)
                .OrderBy(e => e.OverlapScore)
                .Select(e =>
                    new SequenceVertexPath()
                    {
                        Current = e.Target,
                        Path = v.Path.Append(e.Source).ToArray()
                    }
                ).ToList();

            foreach (var e in newEdges2)
                que.Push(e);
        }

        return null;
    }
}

class SequenceVertexPath
{
    public SequenceVertex[] Path { get; set; }
    public SequenceVertex Current { get; set; }
}