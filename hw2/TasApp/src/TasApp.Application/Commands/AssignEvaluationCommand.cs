namespace TasApp.Application.Commands;

public sealed record AssignEvaluationCommand(
    Guid StudentId,
    Guid CriterionId,
    decimal Score,
    string? Comment);