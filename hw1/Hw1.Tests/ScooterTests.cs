using Xunit;
using Hw1.Domain;

namespace Hw1.Tests;

public class ScooterTests
{
    [Fact]
    public void Constructor_NegativeEnergy_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Scooter("001", "Xiaomi", 8, -1.0));
    }

    [Fact]
    public void GetEnergyUsage_ValidValue_ShouldReturnStoredValue()
    {
        // Arrange
        var scooter = new Scooter("001", "Xiaomi", 8, 3.14);

        // Act & Assert
        Assert.Equal(3.14, scooter.GetEnergyUsage());
    }
}