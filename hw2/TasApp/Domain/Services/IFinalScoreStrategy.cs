using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Services;

/// <summary>
/// Сменяемое правило расчёта итога.
/// Strategy: разные стратегии (взвешенная сумма, процент, строгий расчёт).
/// </summary>
public interface IFinalScoreStrategy
{
    string Name { get; }

    /// <summary>
    /// Посчитать итог по набору оценок и текущему рубрикатору.
    /// </summary>
    FinalScore Calculate(
        IReadOnlyCollection<Evaluation> evaluations,
        Rubricator rubricator);
}