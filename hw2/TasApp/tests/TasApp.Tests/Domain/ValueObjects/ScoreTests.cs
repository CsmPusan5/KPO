using FluentAssertions;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.ValueObjects;

public sealed class ScoreTests
{
    [Fact]
    public void Ctor_WithZero_ShouldAcceptValue()
    {
        // Arrange
        const decimal value = 0m;

        // Act
        var score = new Score(value);

        // Assert
        score.Value.Should().Be(0m);
    }

    [Fact]
    public void Ctor_WithPositiveValue_ShouldAcceptValue()
    {
        // Arrange
        const decimal value = 5.5m;

        // Act
        var score = new Score(value);

        // Assert
        score.Value.Should().Be(5.5m);
    }

    [Fact]
    public void Ctor_WithNegativeValue_ShouldThrowScoreOutOfRange()
    {
        // Arrange
        const decimal value = -0.01m;

        // Act
        Action act = () => new Score(value);

        // Assert
        act.Should().Throw<ScoreOutOfRangeException>();
    }
}