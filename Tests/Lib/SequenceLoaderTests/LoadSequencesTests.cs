using System.Text;
using Lib.Entities;

namespace Tests.Lib.SequenceLoaderTests;

public class LoadSequencesTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRandomSequenceGenerator> _randomSequenceGeneratorMock;
    private readonly SequenceLoader _sequenceLoader;

    public LoadSequencesTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _randomSequenceGeneratorMock = _fixture.Freeze<Mock<IRandomSequenceGenerator>>();

        _sequenceLoader = _fixture.Create<SequenceLoader>();
    }

    [Fact]
    public void LoadSequences_When_Called_Returns_Sequences()
    {
        // Arrange
        var data = $"""
            ; random comment
            >SEQ1
            ATGC
            AATT
            GC
            >SEQ2
            ATGC
            AAC

        """;

        // Act
        var result = _sequenceLoader.LoadSequencesFromFastaStream(new MemoryStream(Encoding.UTF8.GetBytes(data)));

        // Assert
        result.Should().BeEquivalentTo(new Sequence[]
        {
            new
            (new int[]
                {
                    Sequence.A, Sequence.T, Sequence.G, Sequence.C, Sequence.A, Sequence.A, Sequence.T, Sequence.T,
                    Sequence.G, Sequence.C
                }, name: "SEQ1"
            ),
            new(
                new int[]
                {
                    Sequence.A, Sequence.T, Sequence.G, Sequence.C, Sequence.A, Sequence.A, Sequence.C
                }, name: "SEQ2")
        });
    }
}