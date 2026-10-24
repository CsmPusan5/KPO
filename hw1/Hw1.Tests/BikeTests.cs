using Xunit;
using Hw1.Domain;

namespace Hw1.Tests;

public class BikeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyInventoryNumber_ShouldThrowArgumentException(string? number)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Bike(number!, "Bike", 7));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyName_ShouldThrowArgumentException(string? name)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Bike("001", name!, 7));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Constructor_EasyScoreOutOfRange_ShouldThrowArgumentOutOfRangeException(int score)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Bike("001", "Bike", score));
    }

    [Theory]
    [InlineData(1,  false)]
    [InlineData(5,  false)]
    [InlineData(6,  true)]
    [InlineData(10, true)]
    public void IsEasyForBeginner_EasyScore_ShouldReturnExpectedResult(int score, bool expected)
    {
        // Arrange
        var bike = new Bike("001", "Bike", score);

        // Act & Assert
        Assert.Equal(expected, bike.IsEasyForBeginners());
    }

    [Fact]
    public void Constructor_ValidArgs_ShouldStoreValues()
    {
        // Arrange & Act
        var bike = new Bike("001", "Bike", 3);

        // Assert
        Assert.Equal("001", bike.GetInventoryNumber());
        Assert.Equal("Bike", bike.GetName());
        Assert.Equal(3, bike.GetEasyScore());
    }
}