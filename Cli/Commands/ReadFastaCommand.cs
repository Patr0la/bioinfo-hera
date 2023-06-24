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
    private readonly IFileLoader _fileLoader;

    public ReadFastaCommand(IFileLoader fileLoader)
    {
        _fileLoader = fileLoader;
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

        var graph = _fileLoader.LoadPaf(rr, cr);

        console.Output.WriteLine(graph.EdgeCount);

        
        var ctg1PathOv = GraphExtension.ApproachOne(graph, "ctg1");
        var ctg2PathOv = GraphExtension.ApproachOne(graph, "ctg2");
        var ctg3PathOv = GraphExtension.ApproachOne(graph, "ctg3");
        
        var ctg1PathEx = GraphExtension.ApproachTwo(graph, "ctg1");
        var ctg2PathEx = GraphExtension.ApproachTwo(graph, "ctg2");
        var ctg3PathEx = GraphExtension.ApproachTwo(graph, "ctg3");
        
        var random = new Random(42);
        
        var ctg1PathMnt = GraphExtension.ApproachTree(graph, "ctg1", random);
        var ctg2PathMnt = GraphExtension.ApproachTree(graph, "ctg2", random);
        var ctg3PathMnt = GraphExtension.ApproachTree(graph, "ctg3", random);
        
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
}