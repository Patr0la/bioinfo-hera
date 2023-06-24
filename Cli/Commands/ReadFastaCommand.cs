using System.Text;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Lib.Entities;
using Lib.Interfaces;
using Lib.Services;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Search;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace Cli.Commands;

[Command("read", Description = "Reads a fasta file and prints the sequences.")]
public class ReadFastaCommand : ICommand
{
    private readonly IPafIO _pafIo;
    private readonly IFastaIO _fastaIo;
    private readonly ISequenceBuilder _sequenceBuilder;

    public ReadFastaCommand(IPafIO pafIo, ISequenceBuilder sequenceBuilder, IFastaIO fastaIo)
    {
        _pafIo = pafIo;
        _sequenceBuilder = sequenceBuilder;
        _fastaIo = fastaIo;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var g = new BidirectionalGraph<SequenceVertex, SequenceEdge>();
        var ctg1 = new SequenceVertex() { Name = "ctg1" };
        var v2 = new SequenceVertex() { Name = "b" };
        var v3 = new SequenceVertex() { Name = "c" };
        var v4 = new SequenceVertex() { Name = "d" };
        g.AddVertex(ctg1);
        g.AddVertex(v2);
        g.AddVertex(v3);
        g.AddVertex(v4);
        g.AddEdge(new SequenceEdge() { Source = ctg1, Target = v2, OverlapScore = 10 });
        g.AddEdge(new SequenceEdge() { Source = ctg1, Target = v3, OverlapScore = 20 });
        g.AddEdge(new SequenceEdge() { Source = ctg1, Target = v4, OverlapScore = 30 });
        g.AddEdge(new SequenceEdge() { Source = v2, Target = v3, OverlapScore = 40 });
        g.AddEdge(new SequenceEdge() { Source = v2, Target = v4, OverlapScore = 50 });
        g.AddEdge(new SequenceEdge() { Source = v3, Target = v4, OverlapScore = 60 });
        console.Output.WriteLine(g.EdgeCount);


        var rr = @"/home/patrik/Downloads/pythonProject1(1)/overlapsRR.paf";
        var cr = @"/home/patrik/Downloads/pythonProject1(1)/overlapsCR.paf";
        var reads = @"/home/patrik/Downloads/EColi - synthetic/ecoli_test_reads.fasta";
        var contigs = @"/home/patrik/Downloads/EColi - synthetic/ecoli_test_contigs.fasta";

        var graph = _pafIo.LoadPaf(rr, cr);

        console.Output.WriteLine(graph.EdgeCount);


        var ctg1PathOv = GraphExtension.ApproachOne(graph, "ctg1", e => e.OverlapScore);
        var ctg2PathOv = GraphExtension.ApproachOne(graph, "ctg2", e => e.OverlapScore);
        var ctg3PathOv = GraphExtension.ApproachOne(graph, "ctg3", e => e.OverlapScore);

        var ctg1PathEx = GraphExtension.ApproachOne(graph, "ctg1", e => e.ExtensionScore);
        var ctg2PathEx = GraphExtension.ApproachOne(graph, "ctg2", e => e.ExtensionScore);
        var ctg3PathEx = GraphExtension.ApproachOne(graph, "ctg3", e => e.ExtensionScore);

        var random = new Random(42);

        var ctg1PathMnt = GraphExtension.ApproachTree(graph, "ctg1", random);
        var ctg2PathMnt = GraphExtension.ApproachTree(graph, "ctg2", random);
        var ctg3PathMnt = GraphExtension.ApproachTree(graph, "ctg3", random);

        var contigsLoaded = _fastaIo.LoadFasta(contigs);
        var sequences = _fastaIo.LoadFasta(reads);

        _fastaIo.SaveFasta("ov.fasta", _sequenceBuilder.DebugBuild(
            contigsLoaded["ctg1"],
            ctg1PathOv,
            contigsLoaded["ctg2"],
            ctg2PathOv,
            contigsLoaded["ctg3"],
            sequences
        ));

        _fastaIo.SaveFasta("ex.fasta", _sequenceBuilder.DebugBuild(
            contigsLoaded["ctg1"],
            ctg1PathEx,
            contigsLoaded["ctg2"],
            ctg2PathEx,
            contigsLoaded["ctg3"],
            sequences
        ));

        _fastaIo.SaveFasta("mnt.fasta", _sequenceBuilder.DebugBuild(
            contigsLoaded["ctg1"],
            ctg1PathMnt,
            contigsLoaded["ctg2"],
            ctg2PathMnt,
            contigsLoaded["ctg3"],
            sequences
        ));


        var dotGraph = graph.ToGraphviz(algorithm =>
        {
            algorithm.CommonVertexFormat.Shape = GraphvizVertexShape.Diamond;
            algorithm.CommonEdgeFormat.ToolTip = "Edge tooltip";
            algorithm.FormatVertex += (sender, args) =>
            {
                args.VertexFormat.Label = $"Vertex {args.Vertex.Name}";
                args.VertexFormat.Size = args.Vertex.IsAnchor ? new GraphvizSizeF(50, 50) : new GraphvizSizeF(10, 10);
                args.VertexFormat.FillColor = args.Vertex.IsAnchor ? GraphvizColor.Red : GraphvizColor.Blue;
            };
            algorithm.FormatEdge += (sender, args) =>
            {
                args.EdgeFormat.Label.Value = $"Edge {args.Edge.Source.Name} -> {args.Edge.Target.Name}";
                args.EdgeFormat.Length = 15;
            };
        });

        File.WriteAllText("out.dot", dotGraph);

        return default;
    }

    private static void OutputFasta(string name, Dictionary<string, Sequence> contigsLoaded, Sequence pathCtg1PathOv,
        Sequence pathCtg2PathOv)
    {
        var sb = new StringBuilder();

        sb.AppendLine(">ctg1");
        sb.AppendLine(contigsLoaded["ctg1"].Data);
        sb.AppendLine(">ctg1PathOv");
        sb.AppendLine(pathCtg1PathOv.Data);
        sb.AppendLine(">ctg2");
        sb.AppendLine(contigsLoaded["ctg2"].Data);
        sb.AppendLine(">ctg2PathOv");
        sb.AppendLine(pathCtg2PathOv.Data);
        sb.AppendLine(">ctg3");
        sb.AppendLine(contigsLoaded["ctg3"].Data);


        File.WriteAllText(name, sb.ToString());
    }
}