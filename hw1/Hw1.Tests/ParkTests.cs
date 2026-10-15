using Xunit;
using NSubstitute;
using Hw1.Domain;

namespace Hw1.Tests;

public class ParkTests
{
    private static Park CreatePark(bool serviceCenterResult = true)
    {
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Check(Arg.Any<IInventoryItem>()).Returns(serviceCenterResult);
        return new Park(serviceCenter);
    }

    [Fact]
    public void AddItem_ServiceCenterAccepts_ReturnsAccepted()
    {
        //Arrange
        var park = CreatePark();

        //Act
        var result = park.AddItem(new Bike("001", "BikeTest", 3));

        //Assert
        Assert.Equal(AddResult.Accepted, result);
        Assert.Single(park.GetItems());
    }

    [Fact]
    public void AddItem_ServiceCenterRejects_ReturnsRejected()
    {
        //Arrange
        var park = CreatePark(false);

        //Act
        var result = park.AddItem(new Bike("001", "B", 3));

        //Assert
        Assert.Equal(AddResult.Rejected, result);
        Assert.Empty(park.GetItems());
    }

    [Fact]
    public void AddItem_TryAddDuplicateNubmer_ReturnsDuplicateNumber()
    {
        //Arrange
        var park = CreatePark();
        park.AddItem(new Bike("001", "Bike1", 3));

        //Act
        var result = park.AddItem(new Bike("001", "Bike2", 3));

        //Assert
        Assert.Equal(AddResult.DuplicateNumber, result);
        Assert.Single(park.GetItems());
    }
    

    [Fact]
    public void GetEnergyUsage_NoItems_ReturnsZero()
    {
        //Arrange
        var park = CreatePark();

        //Act
        var totalEnergy = park.GetEnergyUsage();

        //Assert
        Assert.Equal(0, totalEnergy);
    }

    [Fact]
    public void GetEnergyUsage_NoItemsWithEnergyUsage_ReturnsZero()
    {
        //Arrange
        var park = CreatePark();
        park.AddItem(new Bike("001", "Bike", 3));

        //Act
        var totalEnergy = park.GetEnergyUsage();

        //Assert
        Assert.Equal(0, totalEnergy);
    }

    [Fact]
    public void GetEnergyUsage_HaEnergyUsageItems_ReturnsNonZero()
    {
        //Arrange
        var park = CreatePark();
        park.AddItem(new Scooter("002", "Scooter", 7, 12.57));
        park.AddItem(new ChargeBattery("005", "Battery", 10, 9.81));

        //Act
        var totalEnergy = park.GetEnergyUsage();

        //Assert
        Assert.Equal(22.38, totalEnergy, precision: 3);
    }

    [Fact]
    public void GetEasyForBeginners_MixedScores_ShouldReturnOnlyEasy()
    {
        // Arrange
        var park = CreatePark();
        park.AddItem(new Bike("001", "Easy", 8));
        park.AddItem(new Bike("002", "Hard", 3));
        park.AddItem(new Scooter("003", "Mid", 6, 1.0));

        // Act
        var result = park.GetEasyForBeginners();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.GetInventoryNumber() == "001");
        Assert.Contains(result, x => x.GetInventoryNumber() == "003");
    }

    [Fact]
    public void GetItems_ChageResult_ShouldNotAffectPark()
    {
        // Arrange
        var park = CreatePark();
        park.AddItem(new Bike("001", "Bike", 7));

        // Act
        park.GetItems().Clear();

        // Assert
        Assert.Single(park.GetItems());
    }
}
