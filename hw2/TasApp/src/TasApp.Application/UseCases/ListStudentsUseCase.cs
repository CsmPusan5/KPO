using TasApp.Application.Dtos;
using TasApp.Domain.Ports;

namespace TasApp.Application.UseCases;

public sealed class ListStudentsUseCase
{
    private readonly IStudentRepository _students;

    public ListStudentsUseCase(IStudentRepository students) => _students = students;

    public async Task<IReadOnlyList<StudentDto>> ExecuteAsync(CancellationToken ct = default)
    {
        var students = await _students.ListAsync(ct);
        return students
            .Select(s => new StudentDto(s.Id.Value, s.Name.Value))
            .ToList();
    }
}