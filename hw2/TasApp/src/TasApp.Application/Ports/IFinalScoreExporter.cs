using TasApp.Application.Dtos;

namespace TasApp.Application.Ports;

public interface IFinalScoreExporter
{
    Task ExportAsync(FinalScoreDto dto, string filePath, CancellationToken ct = default);
    Task ExportManyAsync(IReadOnlyList<FinalScoreDto> dtos, string filePath, CancellationToken ct = default);
}