namespace TasApp.Application.Commands;

public sealed record UpdateCriterionCommand(
    Guid CriterionId,
    string NewName,
    decimal NewMaxScore,
    decimal NewWeight);