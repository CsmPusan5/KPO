namespace TasApp.Application.Dtos;

public sealed record CriterionDto(Guid Id, string Name, decimal MaxScore, decimal Weight);