namespace TasApp.Domain.Exceptions;

public sealed class DuplicateCriterionNameException : DomainException
{
    public IReadOnlyList<string> DuplicateNames { get; }

    public DuplicateCriterionNameException(IReadOnlyList<string> duplicates)
        : base($"Duplicate criterion names: {string.Join(", ", duplicates)}")
    {
        DuplicateNames = duplicates;
    }
}