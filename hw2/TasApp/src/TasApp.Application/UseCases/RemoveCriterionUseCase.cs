using TasApp.Application.Commands;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class RemoveCriterionUseCase
{
    private readonly IRubricatorRepository _rubricators;

    public RemoveCriterionUseCase(IRubricatorRepository rubricators) => _rubricators = rubricators;

    public async Task ExecuteAsync(RemoveCriterionCommand cmd, CancellationToken ct = default)
    {
        var rubricator = await _rubricators.GetAsync(ct);
        rubricator.RemoveCriterion(new CriterionId(cmd.CriterionId));
        await _rubricators.SaveAsync(rubricator, ct);
    }
}