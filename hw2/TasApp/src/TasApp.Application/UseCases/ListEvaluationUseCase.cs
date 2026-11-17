using TasApp.Application.Dtos;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class ListEvaluationsUseCase
{
    private readonly IEvaluationRepository _evaluations;
    private readonly IRubricatorRepository _rubricators;

    public ListEvaluationsUseCase(
        IEvaluationRepository evaluations,
        IRubricatorRepository rubricators)
    {
        _evaluations = evaluations;
        _rubricators = rubricators;
    }

    public async Task<IReadOnlyList<EvaluationDto>> ExecuteAsync(Guid studentId, CancellationToken ct = default)
    {
        var evaluations = await _evaluations.ListByStudentAsync(new StudentId(studentId), ct);
        if (evaluations.Count == 0)
            return Array.Empty<EvaluationDto>();

        var rubricator = await _rubricators.GetAsync(ct);
        var criteriaById = rubricator.Criteria.ToDictionary(c => c.Id.Value, c => c.Name.Value);

        return evaluations.Select(e => new EvaluationDto(
            e.Id.Value,
            e.StudentId.Value,
            e.CriterionId.Value,
            criteriaById.TryGetValue(e.CriterionId.Value, out var name) ? name : "<удалён>",
            e.Score.Value,
            e.AssignedAt,
            e.Comment)).ToList();
    }
}