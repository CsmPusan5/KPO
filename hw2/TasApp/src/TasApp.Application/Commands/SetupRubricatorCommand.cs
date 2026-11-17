namespace TasApp.Application.Commands;

public sealed record SetupRubricatorCommand(IReadOnlyList<SetupCriterionCommand> Criteria);