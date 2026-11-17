using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Entities;

public sealed class Criterion
{
    public CriterionId Id { get; }
    public CriterionName Name { get; private set; }
    public MaxScore MaxScore { get; private set; }
    public Weight Weight { get; private set; }

    // internal: Criterion создаётся и изменяется только через Rubricator,
    // чтобы нельзя было обойти инварианты набора критериев.
    internal Criterion(CriterionId id, CriterionName name, MaxScore maxScore, Weight weight)
    {
        Id = id;
        Name = name;
        MaxScore = maxScore;
        Weight = weight;
    }

    internal void Rename(CriterionName newName) => Name = newName;

    internal void ChangeMaxScore(MaxScore newMaxScore) => MaxScore = newMaxScore;

    internal void ChangeWeight(Weight newWeight) => Weight = newWeight;

    public override string ToString()
        => $"Criterion({Id.Value:N}): {Name.Value} [max={MaxScore.Value}, w={Weight.Value}]";
}