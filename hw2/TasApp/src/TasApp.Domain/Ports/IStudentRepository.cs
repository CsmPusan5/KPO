using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Ports;

public interface IStudentRepository
{
    Task AddAsync(Student student, CancellationToken ct = default);
    Task<Student?> GetByIdAsync(StudentId id, CancellationToken ct = default);
    Task<IReadOnlyList<Student>> ListAsync(CancellationToken ct = default);
    Task UpdateAsync(Student student, CancellationToken ct = default);
}