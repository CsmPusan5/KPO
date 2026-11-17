using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Ports;

// GetByIdAsync возвращает null, если запись не найдена. Существование проверяет use case.
public interface IEvaluationRepository
{
    Task AddAsync(Evaluation evaluation, CancellationToken ct = default);
    Task<Evaluation?> GetByIdAsync(EvaluationId id, CancellationToken ct = default);
    Task<Evaluation?> FindByStudentAndCriterionAsync(StudentId studentId, CriterionId criterionId, CancellationToken ct = default);
    Task<IReadOnlyList<Evaluation>> ListByStudentAsync(StudentId studentId, CancellationToken ct = default);
    Task UpdateAsync(Evaluation evaluation, CancellationToken ct = default);
    Task DeleteAsync(EvaluationId id, CancellationToken ct = default);
}