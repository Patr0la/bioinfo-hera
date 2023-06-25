using System.Collections.Concurrent;
using System.Diagnostics;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Lib.Entities;
using Lib.Interfaces;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace Cli.Commands;

[Command("improve", Description = "Improves contigis joined by repetitive reads.")]
public class ImproveReads : ICommand
{
    private readonly IPafIO _pafIo;
    private readonly IFastaIO _fastaIo;
    private readonly ISequenceBuilder _sequenceBuilder;
    private readonly IGraphExtender _graphExtender;
    private readonly IConsensusBuilder _consensusBuilder;

    public ImproveReads(IPafIO pafIo,
        IFastaIO fastaIo,
        ISequenceBuilder sequenceBuilder,
        IGraphExtender graphExtender,
        IConsensusBuilder consensusBuilder)
    {
        _pafIo = pafIo;
        _fastaIo = fastaIo;
        _sequenceBuilder = sequenceBuilder;
        _graphExtender = graphExtender;
        _consensusBuilder = consensusBuilder;
    }

    [CommandParameter(0, Description = "The path to output file.", IsRequired = true)]
    public string OutputPath { get; set; }

    [CommandOption("reads", 'r', Description = "The path reads file.", IsRequired = true)]
    public string ReadsPath { get; set; }

    [CommandOption("contigs", 'c', Description = "The path to contigs file.", IsRequired = true)]
    public string ContigsPath { get; set; }

    [CommandOption("rr-overlaps", Description = "The path to read-read overlaps file.", IsRequired = true)]
    public string ReadReadOverlapsPath { get; set; }

    [CommandOption("cr-overlaps", Description = "The path to contig-read overlaps file.", IsRequired = true)]
    public string ContigReadOverlapsPath { get; set; }

    [CommandOption("verbose", 'v', Description = "Prints the output to console.")]
    public bool Verbose { get; set; }

    [CommandOption("monte-carlo-repeats", Description = "The number of monte carlo repeats.")]
    public int MonteCarloRepeats { get; set; } = 3000;

    public ValueTask ExecuteAsync(IConsole console)
    {
        var contigs = _fastaIo.LoadFasta(ContigsPath);
        var sequences = _fastaIo.LoadFasta(ReadsPath);

        var graph = _pafIo.LoadPaf(ReadReadOverlapsPath, ContigReadOverlapsPath);

        console.Output.WriteLine(graph.EdgeCount);
        var random = new Random(42);

        var ctgConnectionPaths = new List<ICollection<SequenceEdge>>(contigs.Count);

        var bestPaths = new List<Sequence>();

        foreach (var contig in contigs)
        {
            var contigPaths = new List<ICollection<SequenceEdge>?>();
            contigPaths.AddRange(_graphExtender.DFSByWeight(graph, contig.Key, e => e.OverlapScore));
            contigPaths.AddRange(_graphExtender.DFSByWeight(graph, contig.Key, e => e.ExtensionScore));

            Parallel.ForEach(Partitioner.Create(0, MonteCarloRepeats), range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    contigPaths.Add(_graphExtender.MonteCarloSearch(graph, contig.Key, random, e => e.ExtensionScore));
                }
            });

            var validPaths = contigPaths.Where(p => p != null)
                .Select(p => p!)
                .ToList();

            if (validPaths.Count > 0)
            {
                var bestPath = _consensusBuilder.Concensus(validPaths, sequences);

                bestPaths.Add(bestPath);
            }
        }

        _fastaIo.SaveFasta(OutputPath, _sequenceBuilder.DebugBuild(contigs.Values, bestPaths));

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
                var score = Math.Max(Math.Min(args.Edge.OverlapScore / 50000, 1), 0);
                var value = (byte) (255 * score);
                args.EdgeFormat.StrokeColor = new GraphvizColor(255, value, value, value);
            };
        });

        File.WriteAllText("out.dot", dotGraph);

        return default;
    }
}