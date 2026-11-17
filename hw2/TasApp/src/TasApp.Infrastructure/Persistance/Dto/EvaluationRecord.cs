namespace TasApp.Infrastructure.Persistence.Dto;

internal sealed record EvaluationRecord(
    Guid Id,
    Guid StudentId,
    Guid CriterionId,
    decimal Score,
    DateTimeOffset AssignedAt,
    string? Comment);