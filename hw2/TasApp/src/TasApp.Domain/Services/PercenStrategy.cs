using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Services;

/// <summary>
/// Процент: сумма (score * weight) / сумма (maxScore * weight) * 100.
/// Даёт итог в шкале 0..100 независимо от суммы весов.
/// </summary>
public sealed class PercentStrategy : IFinalScoreStrategy
{
    public string Name => "Percent";

    public FinalScore Calculate(
        IReadOnlyCollection<Evaluation> evaluations,
        Rubricator rubricator)
    {
        ArgumentNullException.ThrowIfNull(evaluations);
        ArgumentNullException.ThrowIfNull(rubricator);

        var byCriterion = evaluations
            .GroupBy(e => e.CriterionId)
            .ToDictionary(g => g.Key, g => g.First());

        decimal numerator = 0m;
        decimal denominator = 0m;

        foreach (var criterion in rubricator.Criteria)
        {
            var weight = criterion.Weight.Value;
            var max = criterion.MaxScore.Value;

            denominator += max * weight;

            if (byCriterion.TryGetValue(criterion.Id, out var evaluation))
                numerator += evaluation.Score.Value * weight;
        }

        // Возвращаем 0, чтобы избежать деления на ноль.
        if (denominator == 0m)
            return new FinalScore(0m);

        var percent = numerator / denominator * 100m;
        return new FinalScore(percent);
    }
}