namespace TasApp.Application.Commands;

public sealed record SetupCriterionCommand(string Name, decimal MaxScore, decimal Weight);