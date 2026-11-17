using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

public sealed class Rubricator
{
    private readonly List<Criterion> _criteria = new();

    public IReadOnlyList<Criterion> Criteria => _criteria;

    public Rubricator() { }

    public Criterion AddCriterion(CriterionName name, MaxScore maxScore, Weight weight)
    {
        var criterion = new Criterion(CriterionId.New(), name, maxScore, weight);
        _criteria.Add(criterion);
        return criterion;
    }

    // Инвариант: оценка ставится только по существующему критерию.
    public Criterion GetCriterion(CriterionId id)
    {
        var criterion = _criteria.FirstOrDefault(c => c.Id == id);
        if (criterion is null)
            throw new CriterionNotFoundInRubricatorException(id);
        return criterion;
    }

    public bool ContainsCriterion(CriterionId id)
        => _criteria.Any(c => c.Id == id);

    // Обновить критерий через корень агрегата.
    public void UpdateCriterion(CriterionId id, CriterionName newName, MaxScore newMaxScore, Weight newWeight)
    {
        var criterion = GetCriterion(id);
        criterion.Rename(newName);
        criterion.ChangeMaxScore(newMaxScore);
        criterion.ChangeWeight(newWeight);
    }

    public void RemoveCriterion(CriterionId id)
    {
        var criterion = GetCriterion(id);
        _criteria.Remove(criterion);
    }

    // Восстановление из хранилища. Наполняет приватный список Criteria в обход публичного API.
    public static Rubricator Rehydrate(IEnumerable<Criterion> criteria)
    {
        var rubricator = new Rubricator();
        rubricator._criteria.AddRange(criteria);
        return rubricator;
    }
}