using System.Globalization;
using TasApp.Application.Dtos;
using TasApp.Application.Ports;

namespace TasApp.Infrastructure.Export;

public sealed class CsvFinalScoreExporter : IFinalScoreExporter
{
    public async Task ExportAsync(FinalScoreDto dto, string filePath, CancellationToken ct = default)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await using var writer = new StreamWriter(filePath);
        await writer.WriteLineAsync("StudentId,FinalScore,Strategy");
        await writer.WriteLineAsync(string.Join(",",
            dto.StudentId.ToString(),
            dto.Value.ToString(CultureInfo.InvariantCulture),
            dto.Strategy));
    }

    public async Task ExportManyAsync(
    IReadOnlyList<FinalScoreDto> dtos, string filePath, CancellationToken ct = default)
{
    var dir = Path.GetDirectoryName(filePath);
    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

    await using var writer = new StreamWriter(filePath);
    await writer.WriteLineAsync("StudentId,FinalScore,Strategy");
    foreach (var dto in dtos)
        await writer.WriteLineAsync(string.Join(",",
            dto.StudentId.ToString(),
            dto.Value.ToString(CultureInfo.InvariantCulture),
            dto.Strategy));
}
}