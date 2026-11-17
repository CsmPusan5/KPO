using TasApp.Application.Commands;
using TasApp.Application.Dtos;
using TasApp.Domain.Factories;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class SetupRubricatorUseCase
{
    private readonly IRubricatorRepository _rubricators;

    public SetupRubricatorUseCase(IRubricatorRepository rubricators) => _rubricators = rubricators;

    public async Task<IReadOnlyList<CriterionDto>> ExecuteAsync(
        SetupRubricatorCommand cmd, CancellationToken ct = default)
    {
        var builder = new RubricatorBuilder();

        foreach (var input in cmd.Criteria)
        {
            builder.AddCriterion(
                new CriterionName(input.Name),
                new MaxScore(input.MaxScore),
                new Weight(input.Weight));
        }

        var rubricator = builder.Build();
        await _rubricators.SaveAsync(rubricator, ct);

        return rubricator.Criteria
            .Select(c => new CriterionDto(c.Id.Value, c.Name.Value, c.MaxScore.Value, c.Weight.Value))
            .ToList();
    }
}