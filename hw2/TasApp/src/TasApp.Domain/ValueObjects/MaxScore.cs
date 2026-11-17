using TasApp.Domain.Exceptions;

namespace TasApp.Domain.ValueObjects;

public readonly record struct MaxScore
{
    public decimal Value { get; }

    public MaxScore(decimal value)
    {
        if (value <= 0)
            throw new InvalidMaxScoreException(value);
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}
