using TasApp.Application.Commands;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class AddStudentUseCase
{
    private readonly IStudentRepository _students;

    public AddStudentUseCase(IStudentRepository students) => _students = students;

    public async Task<StudentId> ExecuteAsync(AddStudentCommand cmd, CancellationToken ct = default)
    {
        var student = Student.Create(new StudentName(cmd.Name));
        await _students.AddAsync(student, ct);
        return student.Id;
    }
}