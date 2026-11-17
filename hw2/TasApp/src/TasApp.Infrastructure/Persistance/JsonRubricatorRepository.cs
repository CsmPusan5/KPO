using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;
using TasApp.Infrastructure.Persistence.Dto;

namespace TasApp.Infrastructure.Persistence;

public sealed class JsonRubricatorRepository : IRubricatorRepository
{
    private readonly JsonFileStore<RubricatorRecord> _store;

    public JsonRubricatorRepository(string filePath)
        => _store = new JsonFileStore<RubricatorRecord>(filePath);

    public async Task<Rubricator> GetAsync(CancellationToken ct = default)
    {
        var record = await _store.LoadAsync(ct);
        // Если файла нет - пустой рубрикатор
        if (record is null)
            return new Rubricator();

        var criteria = record.Criteria.Select(ToCriterionDomain);
        return Rubricator.Rehydrate(criteria);
    }

    public async Task SaveAsync(Rubricator rubricator, CancellationToken ct = default)
    {
        var record = new RubricatorRecord(
            rubricator.Criteria.Select(ToCriterionRecord).ToList());
        await _store.SaveAsync(record, ct);
    }

    private static CriterionRecord ToCriterionRecord(Criterion c)
        => new(c.Id.Value, c.Name.Value, c.MaxScore.Value, c.Weight.Value);

    // Rehydrate для критерия недоступен - используем Rubricator.Rehydrate.
    private static Criterion ToCriterionDomain(CriterionRecord r)
        => new(new CriterionId(r.Id), new CriterionName(r.Name), new MaxScore(r.MaxScore), new Weight(r.Weight));
}