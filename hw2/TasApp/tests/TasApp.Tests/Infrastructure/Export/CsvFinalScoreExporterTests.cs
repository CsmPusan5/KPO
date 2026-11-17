using FluentAssertions;
using TasApp.Application.Dtos;
using TasApp.Infrastructure.Export;

namespace TasApp.Tests.Infrastructure.Export;

public sealed class CsvFinalScoreExporterTests : IDisposable
{
    private readonly string _filePath;
    private readonly CsvFinalScoreExporter _sut;

    public CsvFinalScoreExporterTests()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"report-{Guid.NewGuid():N}.csv");
        _sut = new CsvFinalScoreExporter();
    }

    public void Dispose()
    {
        if (File.Exists(_filePath)) File.Delete(_filePath);
    }

    [Fact]
    public async Task ExportAsync_WithDto_ShouldWriteHeaderAndRow()
    {
        // Arrange
        var dto = new FinalScoreDto(Guid.NewGuid(), 0.86m, "WeightedSum");

        // Act
        await _sut.ExportAsync(dto, _filePath);

        // Assert
        File.Exists(_filePath).Should().BeTrue();
        var lines = await File.ReadAllLinesAsync(_filePath);
        lines.Should().HaveCount(2);
        lines[0].Should().Be("StudentId,FinalScore,Strategy");
    }

    [Fact]
    public async Task ExportAsync_WithDto_ShouldWriteCorrectValues()
    {
        // Arrange
        var studentId = Guid.NewGuid();
        var dto = new FinalScoreDto(studentId, 0.86m, "WeightedSum");

        // Act
        await _sut.ExportAsync(dto, _filePath);

        // Assert
        var lines = await File.ReadAllLinesAsync(_filePath);
        lines[1].Should().Be($"{studentId},0.86,WeightedSum");
    }

    [Fact]
    public async Task ExportAsync_WhenDirectoryMissing_ShouldCreateIt()
    {
        // Arrange
        var nestedDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var nestedPath = Path.Combine(nestedDir, "report.csv");
        var dto = new FinalScoreDto(Guid.NewGuid(), 0.5m, "WeightedSum");

        try
        {
            // Act
            await _sut.ExportAsync(dto, nestedPath);

            // Assert
            File.Exists(nestedPath).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(nestedDir)) Directory.Delete(nestedDir, true);
        }
    }
}