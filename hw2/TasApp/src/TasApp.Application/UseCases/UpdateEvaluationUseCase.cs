using TasApp.Application.Commands;
using TasApp.Application.Exceptions;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class UpdateEvaluationUseCase
{
    private readonly IEvaluationRepository _evaluations;
    private readonly IRubricatorRepository _rubricators;

    public UpdateEvaluationUseCase(
        IEvaluationRepository evaluations,
        IRubricatorRepository rubricators)
    {
        _evaluations = evaluations;
        _rubricators = rubricators;
    }

    public async Task ExecuteAsync(UpdateEvaluationCommand cmd, CancellationToken ct = default)
    {
        var evaluation = await _evaluations.GetByIdAsync(new EvaluationId(cmd.EvaluationId), ct)
                         ?? throw new EvaluationNotFoundException(cmd.EvaluationId);

        var rubricator = await _rubricators.GetAsync(ct);
        var criterion = rubricator.GetCriterion(evaluation.CriterionId);
        if (cmd.NewScore is not null) {
            evaluation.ChangeScore(new Score(cmd.NewScore), criterion.MaxScore);
        }
        evaluation.ChangeComment(cmd.Comment);

        await _evaluations.UpdateAsync(evaluation, ct);
    }
}