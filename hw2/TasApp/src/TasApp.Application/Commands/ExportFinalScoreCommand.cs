namespace TasApp.Application.Commands;

public sealed record ExportFinalScoreCommand(Guid StudentId, string FilePath);