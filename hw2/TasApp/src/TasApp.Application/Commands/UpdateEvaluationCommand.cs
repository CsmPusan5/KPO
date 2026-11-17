namespace TasApp.Application.Commands;

public sealed record UpdateEvaluationCommand(
    Guid EvaluationId,
    decimal? NewScore,
    string? Comment,
    bool ChangeComment);