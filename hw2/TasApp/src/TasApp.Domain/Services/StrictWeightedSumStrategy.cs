using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Services;

// Строгий вариант взвешенной суммы: если хотя бы по одному критерию нет оценки, то итог посчитать нельзя.
public sealed class StrictWeightedSumStrategy : IFinalScoreStrategy
{
    public string Name => "StrictWeightedSum";

    public FinalScore Calculate(
        IReadOnlyCollection<Evaluation> evaluations,
        Rubricator rubricator)
    {
        ArgumentNullException.ThrowIfNull(evaluations);
        ArgumentNullException.ThrowIfNull(rubricator);

        var evaluatedCriterionIds = evaluations
            .Select(e => e.CriterionId)
            .ToHashSet();

        var missing = rubricator.Criteria
            .Where(c => !evaluatedCriterionIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToList();

        if (missing.Count > 0)
            throw new MissingEvaluationsException(missing);

        decimal total = 0m;
        var byCriterion = evaluations.ToDictionary(e => e.CriterionId, e => e);

        foreach (var criterion in rubricator.Criteria)
        {
            var evaluation = byCriterion[criterion.Id];
            var normalized = evaluation.Score.Value / criterion.MaxScore.Value;
            total += normalized * criterion.Weight.Value;
        }

        return new FinalScore(total);
    }
}