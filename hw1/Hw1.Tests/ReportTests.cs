using Xunit;
using Hw1.Application;
using Hw1.Domain;

namespace Hw1.Tests;

public class ReportTests
{
    [Fact]
    public void GenerateReport_ContainsInventoryNumbers()
    {
        // Arrange
        var report = new Report();
        var data = new ReportData();
        data.SetTotalItems(2);
        data.SetTotalEnergy(1.5);
        data.SetTransport([new Bike("001", "Bike", 3)]);
        data.SetThings([new Helmet("002", "Helmet", 9)]);

        // Act
        var text = report.GenerateReport(data);

        // Assert
        Assert.Contains("001", text);
        Assert.Contains("002", text);
        Assert.Contains("Bike", text);
        Assert.Contains("Helmet", text);
    }

    [Fact]
    public void GenerateReport_ContainsTotals()
    {
        // Arrange
        var report = new Report();
        var data = new ReportData();
        data.SetTotalItems(1);
        data.SetTotalEnergy(1.5);
        data.SetTransport([new Scooter("001", "scooter", 8, 1.5)]);
        data.SetThings([]);

        // Act
        var text = report.GenerateReport(data);

        // Assert
        Assert.Contains("1", text);
        Assert.Contains("1.5", text);
    }
}