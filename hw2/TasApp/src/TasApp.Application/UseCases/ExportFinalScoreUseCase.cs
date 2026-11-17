using TasApp.Application.Commands;
using TasApp.Application.Ports;
using TasApp.Application.Queries;

namespace TasApp.Application.UseCases;

public sealed class ExportFinalScoreUseCase
{
    private readonly CalculateFinalScoreUseCase _calculate;
    private readonly IFinalScoreExporter _exporter;

    public ExportFinalScoreUseCase(
        CalculateFinalScoreUseCase calculate,
        IFinalScoreExporter exporter)
    {
        _calculate = calculate;
        _exporter = exporter;
    }

    public async Task ExecuteAsync(ExportFinalScoreCommand cmd, CancellationToken ct = default)
    {
        var dto = await _calculate.ExecuteAsync(new CalculateFinalScoreQuery(cmd.StudentId), ct);
        await _exporter.ExportAsync(dto, cmd.FilePath, ct);
    }
}