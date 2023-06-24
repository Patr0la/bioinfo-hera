using Lib.Entities;
using Lib.Interfaces;
using QuikGraph;
using QuikGraph.Algorithms;

namespace Lib.Services;

public class FileLoader : IFileLoader
{
    public UndirectedGraph<SequenceVertex, SequenceEdge> LoadPaf(string overlapsRRPath, string overlapsCRPath)
    {
        var graph = new UndirectedGraph<SequenceVertex, SequenceEdge>();

        var ignored = new HashSet<string>();

        string? line;
        using var reader2 = new StreamReader(overlapsCRPath);
        while ((line = reader2.ReadLine()) != null)
        {
            var v = GetVertex(line, graph, ignored, true);
            if (v == null) continue;

            var existingEdge = graph
                .AdjacentEdges(v.Target)
                .FirstOrDefault(e => e.Source.IsAnchor);

            if (existingEdge == null)
            {
                graph.AddEdge(v);
                continue;
            }

            if (existingEdge.OverlapScore > v.OverlapScore) continue;

            graph.RemoveEdge(existingEdge);
            graph.AddEdge(v);
        }

        reader2.Close();

        using var reader = new StreamReader(overlapsRRPath);
        while ((line = reader.ReadLine()) != null)
        {
            var e = GetVertex(line, graph, ignored);
            if (e == null) continue;

            graph.AddEdge(e);
        }

        reader.Close();

        /*
        foreach (var v in graph.Vertices)
        {
            if(v.IsAnchor) continue;
            
            var anchors = graph.AdjacentEdges(v)
                .Where(e => e.Target.IsAnchor || e.Source.IsAnchor)
                .ToList();

            if (anchors.Count < 2) continue;

            anchors.Remove(anchors.MaxBy(e => e.OverlapScore));
            graph.RemoveEdges(anchors);
        }
*/
        return graph;
    }

    private SequenceEdge? GetVertex(string pafLine, UndirectedGraph<SequenceVertex, SequenceEdge> graph,
        HashSet<string> ignored,
        bool isContig = false)
    {
        var fields = pafLine.Split('\t');

        var qseqname = fields[0];
        var qlen = int.Parse(fields[1]);
        var qstart = int.Parse(fields[2]);
        var qend = int.Parse(fields[3]);
        var strand = fields[4];
        var tseqname = fields[5];
        var tlen = int.Parse(fields[6]);
        var tstart = int.Parse(fields[7]);
        var tend = int.Parse(fields[8]);
        var resMatches = int.Parse(fields[9]);
        var blockLen = int.Parse(fields[10]);

        var ol1 = qend - qstart;
        var ol2 = tend - tstart;

        var oh1 = qlen - qend;
        var oh2 = tlen - tend;

        var el1 = qstart;
        var el2 = tstart;

        var si = (float)resMatches / blockLen;
        var overlap_score = (ol1 + ol2) * si / 2;

        if (isContig)
        {
            if (qstart > 5000 && qend < qlen - 5000)
            {
                ignored.Add(tseqname);
                return null;
            }
        }
        else
        {
            if(qseqname == tseqname) return null;
            if (ignored.Contains(qseqname) || ignored.Contains(tseqname)) return null;
        }


        var v1 = new SequenceVertex()
        {
            Name = qseqname,
            IsAnchor = isContig,
        };
        var v2 = new SequenceVertex()
        {
            Name = tseqname,
        };
        graph.AddVertex(v1);
        graph.AddVertex(v2);

        return new SequenceEdge()
        {
            Source = v1,
            Target = v2,

            SameStrand = strand == "+",
            OverlapScore = overlap_score,
            ExtensionScore = overlap_score + (float)el2 / 2 - (float)(oh1 + oh2) / 2,
            SequenceIdentity = si,
        };
    }

    public ICollection<Sequence> LoadFasta(string filePath)
    {
        throw new NotImplementedException();
    }
}