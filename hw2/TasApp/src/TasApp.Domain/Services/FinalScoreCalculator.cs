using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Services;

public sealed class FinalScoreCalculator
{
    private readonly IFinalScoreStrategy _strategy;

    public FinalScoreCalculator(IFinalScoreStrategy strategy)
    {
        _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public string StrategyName => _strategy.Name;

    public FinalScore Calculate(
        IReadOnlyCollection<Evaluation> evaluations,
        Rubricator rubricator)
    {
        return _strategy.Calculate(evaluations, rubricator);
    }
}