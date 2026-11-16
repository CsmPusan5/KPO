using TasApp.Domain.Exceptions;

namespace TasApp.Domain.ValueObjects;

public readonly record struct Score
{
    public decimal Value { get; }

    public Score(decimal value)
    {
        if (value < 0)
            throw new ScoreOutOfRangeException($"Score cannot be negative: {value}");
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}