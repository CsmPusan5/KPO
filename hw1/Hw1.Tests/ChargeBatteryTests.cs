using Xunit;
using Hw1.Domain;

namespace Hw1.Tests;

public class ChargeBatteryTests
{
    [Fact]
    public void Constructor_NegativeEnergy_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ChargeBattery("001", "Charger", 9, -0.5));
    }

    [Fact]
    public void GetEnergyUsage_ValidValue_ShouldReturnStoredValue()
    {
        // Arrange
        var battery = new ChargeBattery("001", "Charger", 9, 0.8);

        // Act & Assert
        Assert.Equal(0.8, battery.GetEnergyUsage());
    }
}