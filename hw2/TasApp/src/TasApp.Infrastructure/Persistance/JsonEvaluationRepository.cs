using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;
using TasApp.Infrastructure.Persistence.Dto;

namespace TasApp.Infrastructure.Persistence;

public sealed class JsonEvaluationRepository : IEvaluationRepository
{
    private readonly JsonFileStore<List<EvaluationRecord>> _store;

    public JsonEvaluationRepository(string filePath)
        => _store = new JsonFileStore<List<EvaluationRecord>>(filePath);

    public async Task AddAsync(Evaluation evaluation, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<EvaluationRecord>();
        records.Add(ToRecord(evaluation));
        await _store.SaveAsync(records, ct);
    }

    public async Task<Evaluation?> GetByIdAsync(EvaluationId id, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<EvaluationRecord>();
        var record = records.FirstOrDefault(r => r.Id == id.Value);
        return record is null ? null : ToDomain(record);
    }

    public async Task<Evaluation?> FindByStudentAndCriterionAsync(StudentId studentId, CriterionId criterionId, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<EvaluationRecord>();
        var record = records.FirstOrDefault(r =>
            r.StudentId == studentId.Value && r.CriterionId == criterionId.Value);
        return record is null ? null : ToDomain(record);
    }

    public async Task<IReadOnlyList<Evaluation>> ListByStudentAsync(StudentId studentId, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<EvaluationRecord>();
        return records
            .Where(r => r.StudentId == studentId.Value)
            .Select(ToDomain)
            .ToList();
    }

    public async Task UpdateAsync(Evaluation evaluation, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<EvaluationRecord>();
        var index = records.FindIndex(r => r.Id == evaluation.Id.Value);
        if (index < 0)
            throw new InvalidOperationException($"Evaluation '{evaluation.Id.Value}' not found");

        records[index] = ToRecord(evaluation);
        await _store.SaveAsync(records, ct);
    }

    public async Task DeleteAsync(EvaluationId id, CancellationToken ct = default)
    {
        var records = await _store.LoadAsync(ct) ?? new List<EvaluationRecord>();
        records.RemoveAll(r => r.Id == id.Value);
        await _store.SaveAsync(records, ct);
    }

    private static EvaluationRecord ToRecord(Evaluation e)
        => new(e.Id.Value, e.StudentId.Value, e.CriterionId.Value, e.Score.Value, e.AssignedAt, e.Comment);

    private static Evaluation ToDomain(EvaluationRecord r)
        => Evaluation.Rehydrate(
            new EvaluationId(r.Id),
            new StudentId(r.StudentId),
            new CriterionId(r.CriterionId),
            new Score(r.Score),
            r.AssignedAt,
            r.Comment);
}