using FluentAssertions;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.ValueObjects;

public sealed class MaxScoreTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.5)]
    public void Ctor_WithNonPositiveValue_ShouldThrowInvalidMaxScore(decimal value)
    {
        // Arrange

        // Act
        Action act = () => new MaxScore(value);

        // Assert
        act.Should().Throw<InvalidMaxScoreException>();
    }

    [Fact]
    public void Ctor_WithPositiveValue_ShouldAcceptValue()
    {
        // Arrange
        const decimal value = 10m;

        // Act
        var maxScore = new MaxScore(value);

        // Assert
        maxScore.Value.Should().Be(10m);
    }
}