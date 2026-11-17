using TasApp.Domain.Exceptions;

namespace TasApp.Domain.ValueObjects;

public readonly record struct Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value <= 0)
            throw new InvalidWeightException(value);
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}