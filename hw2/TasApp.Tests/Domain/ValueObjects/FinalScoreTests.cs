using FluentAssertions;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.ValueObjects;

public sealed class FinalScoreTests
{
    [Fact]
    public void Ctor_WithNegativeValue_ShouldThrowInvalidFinalScore()
    {
        // Arrange
        const decimal value = -0.01m;

        // Act
        Action act = () => new FinalScore(value);

        // Assert
        act.Should().Throw<InvalidFinalScoreException>();
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.86, 0.86)]
    public void Ctor_WithNonNegativeValue_ShouldAcceptValue(decimal input, decimal expected)
    {
        // Arrange

        // Act
        var finalScore = new FinalScore(input);

        // Assert
        finalScore.Value.Should().Be(expected);
    }
}