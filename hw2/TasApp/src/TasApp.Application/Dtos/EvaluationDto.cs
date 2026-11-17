namespace TasApp.Application.Dtos;

public sealed record EvaluationDto(
    Guid Id,
    Guid StudentId,
    Guid CriterionId,
    decimal Score,
    DateTimeOffset AssignedAt,
    string? Comment);