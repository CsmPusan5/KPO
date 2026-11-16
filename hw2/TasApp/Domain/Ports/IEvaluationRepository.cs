using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Ports;

public interface IEvaluationRepository
{
    Task AddAsync(Evaluation evaluation, CancellationToken ct = default);
    Task<Evaluation?> GetByIdAsync(EvaluationId id, CancellationToken ct = default);
    Task<IReadOnlyList<Evaluation>> ListByStudentAsync(StudentId studentId, CancellationToken ct = default);
    Task UpdateAsync(Evaluation evaluation, CancellationToken ct = default);
    Task DeleteAsync(EvaluationId id, CancellationToken ct = default);
}