using TasApp.Application.Dtos;
using TasApp.Application.Queries;
using TasApp.Domain.Ports;
using TasApp.Domain.Services;
using TasApp.Domain.ValueObjects;

namespace TasApp.Application.UseCases;

public sealed class CalculateFinalScoreUseCase
{
    private readonly IEvaluationRepository _evaluations;
    private readonly IRubricatorRepository _rubricators;
    private readonly FinalScoreCalculator _calculator;

    public CalculateFinalScoreUseCase(
        IEvaluationRepository evaluations,
        IRubricatorRepository rubricators,
        FinalScoreCalculator calculator)
    {
        _evaluations = evaluations;
        _rubricators = rubricators;
        _calculator = calculator;
    }

    public async Task<FinalScoreDto> ExecuteAsync(CalculateFinalScoreQuery query, CancellationToken ct = default)
    {
        var studentId = new StudentId(query.StudentId);
        var evaluations = await _evaluations.ListByStudentAsync(studentId, ct);
        var rubricator = await _rubricators.GetAsync(ct);

        var finalScore = _calculator.Calculate(evaluations, rubricator);

        return new FinalScoreDto(query.StudentId, finalScore.Value, _calculator.StrategyName);
    }
}