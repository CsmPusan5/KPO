// tests/TasApp.Tests/Domain/ValueObjects/WeightTests.cs
using FluentAssertions;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.ValueObjects;

public sealed class WeightTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Ctor_WithNonPositiveValue_ShouldThrowInvalidWeight(decimal value)
    {
        // Arrange

        // Act
        Action act = () => new Weight(value);

        // Assert
        act.Should().Throw<InvalidWeightException>();
    }

    [Fact]
    public void Ctor_WithPositiveValue_ShouldAcceptValue()
    {
        // Arrange
        const decimal value = 0.4m;

        // Act
        var weight = new Weight(value);

        // Assert
        weight.Value.Should().Be(0.4m);
    }
}