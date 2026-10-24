using Xunit;
using Hw1.Domain;

namespace Hw1.Tests;

public class HelmetTests
{
    [Fact]
    public void Constructor_ValidArgs_ShouldStoreValues()
    {
        // Arrange & Act
        var helmet = new Helmet("001", "Helmet", 9);

        // Assert
        Assert.Equal("001", helmet.GetInventoryNumber());
        Assert.Equal("Helmet", helmet.GetName());
        Assert.Equal(9, helmet.GetEasyScore());
    }
}