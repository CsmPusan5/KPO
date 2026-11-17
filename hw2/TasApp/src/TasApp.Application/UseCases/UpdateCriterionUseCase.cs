using TasApp.Application.Commands;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class UpdateCriterionUseCase
{
    private readonly IRubricatorRepository _rubricators;

    public UpdateCriterionUseCase(IRubricatorRepository rubricators) => _rubricators = rubricators;

    public async Task ExecuteAsync(UpdateCriterionCommand cmd, CancellationToken ct = default)
    {
        var rubricator = await _rubricators.GetAsync(ct);

        rubricator.UpdateCriterion(
            new CriterionId(cmd.CriterionId),
            new CriterionName(cmd.NewName),
            new MaxScore(cmd.NewMaxScore),
            new Weight(cmd.NewWeight));

        await _rubricators.SaveAsync(rubricator, ct);
    }
}