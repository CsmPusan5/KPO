using TasApp.Domain.Entities;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Services;

// Сменяемое правило расчёта итога.
// Strategy: разные стратегии (взвешенная сумма, процент, строгий расчёт).
public interface IFinalScoreStrategy
{
    string Name { get; }

    FinalScore Calculate(
        IReadOnlyCollection<Evaluation> evaluations,
        Rubricator rubricator);
}