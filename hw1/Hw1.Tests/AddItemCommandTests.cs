using Xunit;
using NSubstitute;
using Hw1.Application;
using Hw1.Domain;

namespace Hw1.Tests;

public class AddItemCommandTests
{
    [Theory]
    [InlineData(true, "Принято")] 
    [InlineData(false, "Техосмотр не пройден")]
    public void AddItemCommand_Execute_PrintsCorrectTechInspectionResult(bool serviceCenterResult, string expectedText)
    {
        // Arrange
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Check(Arg.Any<IInventoryItem>()).Returns(serviceCenterResult);
        
        var park = new Park(serviceCenter); 
        var item = new Bike("001", "Bike", 3);
        var command = new AddItemCommand(item);
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        command.Execute(park, Substitute.For<IReport>());

        // Assert
        Assert.Contains(expectedText, consoleOutput.ToString());
        
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }

    [Fact]
    public void AddItemCommand_Execute_PrintsDuplicateError()
    {
        // Arrange
        var serviceCenter = Substitute.For<IServiceCenter>();
        serviceCenter.Check(Arg.Any<IInventoryItem>()).Returns(true);
        var park = new Park(serviceCenter); 
        
        park.AddItem(new Bike("001", "Bike1", 7)); 
        
        var command = new AddItemCommand(new Bike("001", "Bike2", 8));

        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        command.Execute(park, Substitute.For<IReport>());

        // Assert
        Assert.Contains("Дубликат номера", consoleOutput.ToString());
        
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
    }
}