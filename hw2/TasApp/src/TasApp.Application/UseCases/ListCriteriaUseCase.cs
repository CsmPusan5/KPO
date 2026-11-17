using TasApp.Application.Dtos;
using TasApp.Domain.Ports;

namespace TasApp.Application.UseCases;

public sealed class ListCriteriaUseCase
{
    private readonly IRubricatorRepository _rubricators;

    public ListCriteriaUseCase(IRubricatorRepository rubricators) => _rubricators = rubricators;

    public async Task<IReadOnlyList<CriterionDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var rubricator = await _rubricators.GetAsync(ct);
        return rubricator.Criteria
            .Select(c => new CriterionDto(c.Id.Value, c.Name.Value, c.MaxScore.Value, c.Weight.Value))
            .ToList();
    }
}