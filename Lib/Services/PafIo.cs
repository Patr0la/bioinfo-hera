using Lib.Entities;
using Lib.Interfaces;
using QuikGraph;

namespace Lib.Services;

public class PafIo : IPafIO
{
    public BidirectionalGraph<SequenceVertex, SequenceEdge> LoadPaf(string overlapsRRPath, string overlapsCRPath,
        int MinContigOverlap, float MinSequenceIdentity)
    {
        var graph = new BidirectionalGraph<SequenceVertex, SequenceEdge>();

        var ignored = new HashSet<string>();

        string? line;
        using var reader2 = new StreamReader(overlapsCRPath);
        while ((line = reader2.ReadLine()) != null)
        {
            var v = GetVertex(line, graph, ignored, true);
            if (v == null) continue;

            var existingEdge = graph
                .OutEdges(v.Target)
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

        return graph;
    }

    private SequenceEdge? GetVertex(string pafLine, BidirectionalGraph<SequenceVertex, SequenceEdge> graph,
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
        if (si < 0.8)
        {
            return null;
        }

        var overlap_score = (ol1 + ol2) * si / 2;

        if (isContig)
        {
            if (qstart > 2500 && qend < qlen - 2500)
            {
                if (overlap_score > 10000)
                {
                    ignored.Add(tseqname);
                }

                return null;
            }
        }
        else
        {
            if (qseqname == tseqname) return null;
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

            SourceStart = qstart,
            SourceEnd = qend,
            TargetStart = tstart,
            TargetEnd = tend,

            SameStrand = strand == "+",
            OverlapScore = overlap_score,
            ExtensionScore = overlap_score + (float)el2 / 2 - (float)(oh1 + oh2) / 2,
            SequenceIdentity = si,
        };
    }
}