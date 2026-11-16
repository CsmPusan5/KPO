using TasApp.Application.Exceptions;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class DeleteEvaluationUseCase
{
    private readonly IEvaluationRepository _evaluations;

    public DeleteEvaluationUseCase(IEvaluationRepository evaluations) => _evaluations = evaluations;

    public async Task ExecuteAsync(Guid evaluationId, CancellationToken ct = default)
    {
        var evaluation = await _evaluations.GetByIdAsync(new EvaluationId(evaluationId), ct)
                         ?? throw new EvaluationNotFoundException(evaluationId);

        await _evaluations.DeleteAsync(evaluation.Id, ct);
    }
}