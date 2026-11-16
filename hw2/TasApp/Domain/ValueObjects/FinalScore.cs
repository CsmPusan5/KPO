using TasApp.Domain.Exceptions;

namespace TasApp.Domain.ValueObjects;

public readonly record struct FinalScore
{
    public decimal Value { get; }

    public FinalScore(decimal value)
    {
        if (value < 0)
            throw new InvalidFinalScoreException(value);
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}