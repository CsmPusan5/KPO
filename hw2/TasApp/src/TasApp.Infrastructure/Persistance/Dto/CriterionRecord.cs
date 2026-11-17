namespace TasApp.Infrastructure.Persistence.Dto;

internal sealed record CriterionRecord(Guid Id, string Name, decimal MaxScore, decimal Weight);