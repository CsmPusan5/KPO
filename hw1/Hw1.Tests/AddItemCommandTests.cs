using Xunit;
using NSubstitute;
using Hw1.Application;
using Hw1.Domain;

namespace Hw1.Tests;

public class AddItemCommandTests
{
    [Theory]
    [InlineData(AddResult.Accepted)]
    [InlineData(AddResult.Rejected)]
    [InlineData(AddResult.DuplicateNumber)]
    public void Execute_AnyAddResult_ShouldReturnTrue(AddResult result)
    {
        // Arrange
        var park = Substitute.For<Park>(Substitute.For<IServiceCenter>());
        park.AddItem(Arg.Any<IInventoryItem>()).Returns(result);
        var report = Substitute.For<IReport>();
        var command = new AddItemCommand(new Bike("001", "Bike", 7));

        // Act & Assert
        Assert.True(command.Execute(park, report));
    }
}