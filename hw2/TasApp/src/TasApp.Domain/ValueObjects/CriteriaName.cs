using TasApp.Domain.Exceptions;

namespace TasApp.Domain.ValueObjects;

public readonly record struct CriterionName
{
    public string Value { get; }

    public CriterionName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidCriterionNameException(value);
        Value = value.Trim();
    }

    public override string ToString() => Value;
}