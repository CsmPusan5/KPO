namespace TasApp.Domain.ValueObjects;

public readonly record struct StudentId(Guid Value)
{
    public static StudentId New() => new(Guid.NewGuid());
}

public readonly record struct CriterionId(Guid Value)
{
    public static CriterionId New() => new(Guid.NewGuid());
}

public readonly record struct EvaluationId(Guid Value)
{
    public static EvaluationId New() => new(Guid.NewGuid());
}