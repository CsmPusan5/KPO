namespace TasApp.Application.Dtos;

public sealed record FinalScoreDto(Guid StudentId, decimal Value, string Strategy);