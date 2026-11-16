using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Services;

/// <summary>
/// Взвешенная сумма: для каждого критерия берём score/maxScore (0..1),
/// умножаем на вес и суммируем. Отсутствующая оценка = 0.
/// </summary>
public sealed class WeightedSumStrategy : IFinalScoreStrategy
{
    public string Name => "WeightedSum";

    public FinalScore Calculate(
        IReadOnlyCollection<Evaluation> evaluations,
        Rubricator rubricator)
    {
        ArgumentNullException.ThrowIfNull(evaluations);
        ArgumentNullException.ThrowIfNull(rubricator);

        var byCriterion = evaluations
            .GroupBy(e => e.CriterionId)
            .ToDictionary(g => g.Key, g => g.First());

        decimal total = 0m;

        foreach (var criterion in rubricator.Criteria)
        {
            if (!byCriterion.TryGetValue(criterion.Id, out var evaluation))
                continue; // отсутствие оценки = 0 вклада

            var normalized = evaluation.Score.Value / criterion.MaxScore.Value; // 0..1
            total += normalized * criterion.Weight.Value;
        }

        return new FinalScore(total);
    }
}