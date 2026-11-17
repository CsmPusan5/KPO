using TasApp.Application.Commands;
using TasApp.Application.Exceptions;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class RenameStudentUseCase
{
    private readonly IStudentRepository _students;

    public RenameStudentUseCase(IStudentRepository students) => _students = students;

    public async Task ExecuteAsync(RenameStudentCommand cmd, CancellationToken ct = default)
    {
        var student = await _students.GetByIdAsync(new StudentId(cmd.StudentId), ct)
                      ?? throw new StudentNotFoundException(cmd.StudentId);

        student.Rename(new StudentName(cmd.NewName));
        await _students.UpdateAsync(student, ct);
    }
}