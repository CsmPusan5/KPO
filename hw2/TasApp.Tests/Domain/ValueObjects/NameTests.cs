using FluentAssertions;
using TasApp.Domain.Exceptions;
using TasApp.Domain.ValueObjects;

namespace TasApp.Tests.Domain.ValueObjects;

public sealed class NameTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void StudentName_WithEmptyValue_ShouldThrowInvalidStudentName(string? value)
    {
        // Arrange

        // Act
        Action act = () => new StudentName(value!);

        // Assert
        act.Should().Throw<InvalidStudentNameException>();
    }

    [Fact]
    public void StudentName_WithSurroundingWhitespace_ShouldTrimValue()
    {
        // Arrange
        const string raw = "  Ivan  ";

        // Act
        var name = new StudentName(raw);

        // Assert
        name.Value.Should().Be("Ivan");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CriterionName_WithEmptyValue_ShouldThrowInvalidCriterionName(string? value)
    {
        // Arrange

        // Act
        Action act = () => new CriterionName(value!);

        // Assert
        act.Should().Throw<InvalidCriterionNameException>();
    }

    [Fact]
    public void CriterionName_WithSurroundingWhitespace_ShouldTrimValue()
    {
        // Arrange
        const string raw = "  ДЗ  ";

        // Act
        var name = new CriterionName(raw);

        // Assert
        name.Value.Should().Be("ДЗ");
    }
}