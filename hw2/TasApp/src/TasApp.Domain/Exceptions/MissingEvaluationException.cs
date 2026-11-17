namespace TasApp.Domain.Exceptions;

public sealed class MissingEvaluationsException : DomainException
{
    public IReadOnlyList<ValueObjects.CriterionId> MissingCriterionIds { get; }

    public MissingEvaluationsException(IReadOnlyList<ValueObjects.CriterionId> missing)
        : base($"Missing evaluations for criteria: {string.Join(", ", missing)}")
    {
        MissingCriterionIds = missing;
    }
}