using TasApp.Domain.Exceptions;

namespace TasApp.Domain.ValueObjects;

public readonly record struct StudentName
{
    public string Value { get; }

    public StudentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidStudentNameException(value);
        Value = value.Trim();
    }

    public override string ToString() => Value;
}