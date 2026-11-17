using TasApp.Domain.ValueObjects;

namespace TasApp.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception inner) : base(message, inner) { }
}

public sealed class InvalidStudentNameException : DomainException
{
    public InvalidStudentNameException(string? value)
        : base($"Student name must be non-empty, got: '{value}'") { }
}

public sealed class InvalidCriterionNameException : DomainException
{
    public InvalidCriterionNameException(string? value)
        : base($"Criterion name must be non-empty, got: '{value}'") { }
}

public sealed class InvalidMaxScoreException : DomainException
{
    public InvalidMaxScoreException(decimal value)
        : base($"MaxScore must be > 0, got: {value}") { }
}

public sealed class InvalidWeightException : DomainException
{
    public InvalidWeightException(decimal value)
        : base($"Weight must be > 0, got: {value}") { }
}

public sealed class ScoreOutOfRangeException : DomainException
{
    public ScoreOutOfRangeException(string message) : base(message) { }

    public static ScoreOutOfRangeException ExceedsMax(decimal score, decimal maxScore)
        => new($"Score {score} exceeds max {maxScore}");
}

public sealed class CriterionNotFoundInRubricatorException : DomainException
{
    public CriterionNotFoundInRubricatorException(CriterionId id)
        : base($"Criterion '{id.Value}' is not present in the rubricator") { }
}

public sealed class InvalidFinalScoreException : DomainException
{
    public InvalidFinalScoreException(decimal value)
        : base($"FinalScore must be >= 0, got: {value}") { }
}