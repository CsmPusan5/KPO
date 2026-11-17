using TasApp.Application.Commands;
using TasApp.Application.Dtos;
using TasApp.Application.Ports;
using TasApp.Application.Queries;

namespace TasApp.Application.UseCases;

public sealed class ExportAllFinalScoresUseCase
{
    private readonly ListStudentsUseCase _listStudents;
    private readonly CalculateFinalScoreUseCase _calculate;
    private readonly IFinalScoreExporter _exporter;

    public ExportAllFinalScoresUseCase(
        ListStudentsUseCase listStudents,
        CalculateFinalScoreUseCase calculate,
        IFinalScoreExporter exporter)
    {
        _listStudents = listStudents;
        _calculate = calculate;
        _exporter = exporter;
    }

    public async Task<int> ExecuteAsync(string filePath, CancellationToken ct = default)
    {
        var students = await _listStudents.ExecuteAsync(ct);
        if (students.Count == 0) return 0;

        var all = new List<FinalScoreDto>(students.Count);
        foreach (var s in students)
            all.Add(await _calculate.ExecuteAsync(new CalculateFinalScoreQuery(s.Id), ct));

        await _exporter.ExportManyAsync(all, filePath, ct);
        return all.Count;
    }
}