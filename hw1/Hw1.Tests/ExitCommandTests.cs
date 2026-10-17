using Xunit;
using NSubstitute;
using Hw1.Application;
using Hw1.Domain;

namespace Hw1.Tests;

public class ExitCommandTests
{
    [Fact]
    public void Execute_Call_ShouldReturnFalse()
    {
        // Arrange
        var park = new Park(Substitute.For<IServiceCenter>());
        var report = Substitute.For<IReport>();
        var command = new ExitCommand();

        // Act & Assert
        Assert.False(command.Execute(park, report));
    }
}