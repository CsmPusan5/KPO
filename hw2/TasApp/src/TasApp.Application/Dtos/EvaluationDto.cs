namespace TasApp.Application.Dtos;

public sealed record EvaluationDto(
    Guid Id,
    Guid StudentId,
    Guid CriterionId,
    string CriterionName,
    decimal Score,
    DateTimeOffset AssignedAt,
    string? Comment);