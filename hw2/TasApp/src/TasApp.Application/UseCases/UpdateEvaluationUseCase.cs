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
        _evaluations = evaluations ?? throw new ArgumentNullException(nameof(evaluations));
        _rubricators = rubricators ?? throw new ArgumentNullException(nameof(rubricators));
    }

    public async Task ExecuteAsync(UpdateEvaluationCommand cmd, CancellationToken ct = default)
    {
        var evaluation = await _evaluations.GetByIdAsync(new EvaluationId(cmd.EvaluationId), ct)
                         ?? throw new EvaluationNotFoundException(cmd.EvaluationId);

        if (cmd.NewScore is { } newScoreValue)
        {
            var rubricator = await _rubricators.GetAsync(ct);
            var criterion = rubricator.GetCriterion(evaluation.CriterionId);
            evaluation.ChangeScore(new Score(newScoreValue), criterion.MaxScore);
        }

        if (cmd.ChangeComment)
        {
            evaluation.ChangeComment(cmd.Comment);
        }

        await _evaluations.UpdateAsync(evaluation, ct);
    }
}