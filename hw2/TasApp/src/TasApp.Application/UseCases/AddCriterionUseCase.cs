using TasApp.Application.Commands;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class AddCriterionUseCase
{
    private readonly IRubricatorRepository _rubricators;

    public AddCriterionUseCase(IRubricatorRepository rubricators) => _rubricators = rubricators;

    public async Task<CriterionId> ExecuteAsync(AddCriterionCommand cmd, CancellationToken ct = default)
    {
        var rubricator = await _rubricators.GetAsync(ct);

        var criterion = rubricator.AddCriterion(
            new CriterionName(cmd.Name),
            new MaxScore(cmd.MaxScore),
            new Weight(cmd.Weight));

        await _rubricators.SaveAsync(rubricator, ct);
        return criterion.Id;
    }
}