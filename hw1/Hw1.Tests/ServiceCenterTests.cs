using Xunit;
using Hw1.Domain;

namespace Hw1.Tests;

public class ServiceCenterTests
{
    private readonly ServiceCenter _serviceCenter = new();

    [Theory]
    [InlineData("2s",    false)]
    [InlineData("3sy",   true)] 
    [InlineData("20 symbols blablabla", true)]
    [InlineData("21 symbols blablablab", false)]
    public void Check_NameLength_ShouldReturnExpectedResult(string name, bool expected)
    {
        // Arrange
        var bike = new Bike("007", name, 7);

        // Act & Assert
        Assert.Equal(expected, _serviceCenter.Check(bike));
    }
}