using System.Collections.Concurrent;
using System.Diagnostics;
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
    private readonly IConcensusBuilder _concensusBuilder;

    public ReadFastaCommand(IPafIO pafIo, ISequenceBuilder sequenceBuilder, IFastaIO fastaIo, IConcensusBuilder concensusBuilder)
    {
        _pafIo = pafIo;
        _sequenceBuilder = sequenceBuilder;
        _fastaIo = fastaIo;
        _concensusBuilder = concensusBuilder;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        var rr = @"/home/patrik/Downloads/pythonProject1(1)/overlapsRR.paf";
        var cr = @"/home/patrik/Downloads/pythonProject1(1)/overlapsCR.paf";
        var reads = @"/home/patrik/Downloads/EColi - synthetic/ecoli_test_reads.fasta";
        var contigs = @"/home/patrik/Downloads/EColi - synthetic/ecoli_test_contigs.fasta";

        var contigsLoaded = _fastaIo.LoadFasta(contigs);
        var sequences = _fastaIo.LoadFasta(reads);
        
        var graph = _pafIo.LoadPaf(rr, cr);

        console.Output.WriteLine(graph.EdgeCount);


        var ctg1PathOv = GraphExtension.DFSByWeight(graph, "ctg1", e => e.OverlapScore);
        var ctg2PathOv = GraphExtension.DFSByWeight(graph, "ctg2", e => e.OverlapScore);
        var ctg3PathOv = GraphExtension.DFSByWeight(graph, "ctg3", e => e.OverlapScore);

        var ctg1PathEx = GraphExtension.DFSByWeight(graph, "ctg1", e => e.ExtensionScore);
        var ctg2PathEx = GraphExtension.DFSByWeight(graph, "ctg2", e => e.ExtensionScore);
        var ctg3PathEx = GraphExtension.DFSByWeight(graph, "ctg3", e => e.ExtensionScore);

        var random = new Random(42);
        
        var MONTE_CARLO_REPEATS = 1000;
        
        var monteCarloCtg1Paths = new List<ICollection<SequenceEdge>>();
        var monteCarloCtg2Paths = new List<ICollection<SequenceEdge>>();
        var monteCarloCtg3Paths = new List<ICollection<SequenceEdge>>();
        
        var time = new Stopwatch();
        time.Start();
        Parallel.ForEach(Partitioner.Create(0, MONTE_CARLO_REPEATS), range =>
        {
            for (int i = range.Item1; i < range.Item2; i++)
            {
                var ctg1PathMnt = GraphExtension.MonteCarloSearch(graph, "ctg1", random);
                monteCarloCtg1Paths.Add(ctg1PathMnt);
            
                var ctg2PathMnt = GraphExtension.MonteCarloSearch(graph, "ctg2", random);
                monteCarloCtg2Paths.Add(ctg2PathMnt);
            
                var ctg3PathMnt = GraphExtension.MonteCarloSearch(graph, "ctg3", random);
                monteCarloCtg3Paths.Add(ctg3PathMnt);
            }
        });
        var endTime = time.ElapsedMilliseconds;
        console.Output.WriteLine($"Monte Carlo paraller with partitioner took {endTime} ms");
        
        /*
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
        ));*/
        
        var allCtg1Ctg1Paths = new List<ICollection<SequenceEdge>>();
        allCtg1Ctg1Paths.Add(ctg1PathOv);
        allCtg1Ctg1Paths.Add(ctg1PathEx);
        allCtg1Ctg1Paths.AddRange(monteCarloCtg1Paths);
        
        _concensusBuilder.Concensus(allCtg1Ctg1Paths, sequences);
        
        var allCtg2Ctg2Paths = new List<ICollection<SequenceEdge>>();
        allCtg2Ctg2Paths.Add(ctg2PathOv);
        allCtg2Ctg2Paths.Add(ctg2PathEx);
        allCtg2Ctg2Paths.AddRange(monteCarloCtg2Paths);
        
        _concensusBuilder.Concensus(allCtg2Ctg2Paths, sequences);
        
        var allCtg3Ctg3Paths = new List<ICollection<SequenceEdge>>();
        allCtg3Ctg3Paths.Add(ctg3PathOv);
        allCtg3Ctg3Paths.Add(ctg3PathEx);
        allCtg3Ctg3Paths.AddRange(monteCarloCtg3Paths);
        

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