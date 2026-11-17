using TasApp.Domain.Entities;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Factories;

// Пошаговая сборка Rubricator с валидацией всего набора в Build().
// при дубликатах имён Build() бросает исключение до создания Rubricator.
public sealed class RubricatorBuilder
{
    private readonly List<(CriterionName Name, MaxScore MaxScore, Weight Weight)> _drafts = new();

    public RubricatorBuilder AddCriterion(CriterionName name, MaxScore maxScore, Weight weight)
    {
        _drafts.Add((name, maxScore, weight));
        return this;
    }

    public Rubricator Build()
    {
        var duplicates = _drafts
            .GroupBy(d => d.Name)   // CriterionName сравнивается по значению
            .Where(g => g.Count() > 1)
            .Select(g => g.Key.Value)
            .ToList();

        if (duplicates.Count > 0)
            throw new DuplicateCriterionNameException(duplicates);

        var rubricator = new Rubricator();
        foreach (var (name, maxScore, weight) in _drafts)
            rubricator.AddCriterion(name, maxScore, weight);

        return rubricator;
    }
}