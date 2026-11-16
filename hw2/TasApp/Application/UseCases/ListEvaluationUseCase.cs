using TasApp.Application.Dtos;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class ListEvaluationsUseCase
{
    private readonly IEvaluationRepository _evaluations;

    public ListEvaluationsUseCase(IEvaluationRepository evaluations) => _evaluations = evaluations;

    public async Task<IReadOnlyList<EvaluationDto>> ExecuteAsync(Guid studentId, CancellationToken ct = default)
    {
        var evaluations = await _evaluations.ListByStudentAsync(new StudentId(studentId), ct);
        return evaluations
            .Select(e => new EvaluationDto(
                e.Id.Value,
                e.StudentId.Value,
                e.CriterionId.Value,
                e.Score.Value,
                e.AssignedAt,
                e.Comment))
            .ToList();
    }
}