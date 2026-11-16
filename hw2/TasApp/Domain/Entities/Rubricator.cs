using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

/// <summary>
/// Рубрикатор — набор критериев. Aggregate Root агрегата «Rubricator + Criterion».
/// </summary>
public sealed class Rubricator
{
    private readonly List<Criterion> _criteria = new();

    public IReadOnlyList<Criterion> Criteria => _criteria;

    // Конструктор без параметров: пустой рубрикатор — валидное состояние.
    public Rubricator() { }

    /// <summary>Добавить критерий. Возвращает созданный объект.</summary>
    public Criterion AddCriterion(CriterionName name, MaxScore maxScore, Weight weight)
    {
        var criterion = new Criterion(CriterionId.New(), name, maxScore, weight);
        _criteria.Add(criterion);
        return criterion;
    }

    /// <summary>Получить критерий по Id. Бросает, если его нет.</summary>
    public Criterion GetCriterion(CriterionId id)
    {
        var criterion = _criteria.FirstOrDefault(c => c.Id == id);
        if (criterion is null)
            throw new CriterionNotFoundInRubricatorException(id);
        return criterion;
    }

    public bool ContainsCriterion(CriterionId id)
        => _criteria.Any(c => c.Id == id);

    /// <summary>Обновить критерий. Только через корень агрегата.</summary>
    public void UpdateCriterion(CriterionId id, CriterionName newName, MaxScore newMaxScore, Weight newWeight)
    {
        var criterion = GetCriterion(id);
        criterion.Rename(newName);
        criterion.ChangeMaxScore(newMaxScore);
        criterion.ChangeWeight(newWeight);
    }

    /// <summary>Удалить критерий.</summary>
    public void RemoveCriterion(CriterionId id)
    {
        var criterion = GetCriterion(id);
        _criteria.Remove(criterion);
    }

    /// <summary>Восстановление из хранилища — критерии создаются через internal конструктор.</summary>
    public static Rubricator Rehydrate(IEnumerable<Criterion> criteria)
    {
        var rubricator = new Rubricator();
        rubricator._criteria.AddRange(criteria);
        return rubricator;
    }
}