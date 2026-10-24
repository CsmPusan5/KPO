using Xunit;
using NSubstitute;
using Hw1.Domain;
using Hw1.Application;

public class ShowItemsCommandTests
{
    [Fact]
    public void ShowItemsCommand_Execute_ReturnsTrue()
    {
        var park = new Park(Substitute.For<IServiceCenter>());
        var report = Substitute.For<IReport>();
        var command = new ShowItemsCommand();
        
        Assert.True(command.Execute(park, report));
}
}