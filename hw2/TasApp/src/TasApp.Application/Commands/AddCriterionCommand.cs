namespace TasApp.Application.Commands;

public sealed record AddCriterionCommand(string Name, decimal MaxScore, decimal Weight);