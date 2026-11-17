using TasApp.Application.Commands;
using TasApp.Application.Exceptions;
using TasApp.Domain.Entities;
using TasApp.Domain.Ports;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class AssignEvaluationUseCase
{
    private readonly IStudentRepository _students;
    private readonly IRubricatorRepository _rubricators;
    private readonly IEvaluationRepository _evaluations;

    public AssignEvaluationUseCase(
        IStudentRepository students,
        IRubricatorRepository rubricators,
        IEvaluationRepository evaluations)
    {
        _students = students;
        _rubricators = rubricators;
        _evaluations = evaluations;
    }

    public async Task<EvaluationId> ExecuteAsync(AssignEvaluationCommand cmd, CancellationToken ct = default)
    {
        var student = await _students.GetByIdAsync(new StudentId(cmd.StudentId), ct)
                      ?? throw new StudentNotFoundException(cmd.StudentId);

        var rubricator = await _rubricators.GetAsync(ct);
        var criterion = rubricator.GetCriterion(new CriterionId(cmd.CriterionId));

        var evaluation = Evaluation.Create(
            student.Id,
            criterion,
            new Score(cmd.Score),
            DateTimeOffset.UtcNow,
            cmd.Comment);

        await _evaluations.AddAsync(evaluation, ct);
        return evaluation.Id;
    }
}