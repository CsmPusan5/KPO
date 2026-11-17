namespace TasApp.Application.Commands;

public sealed record RenameStudentCommand(Guid StudentId, string NewName);