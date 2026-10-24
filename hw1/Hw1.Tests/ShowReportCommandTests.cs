using Xunit;
using NSubstitute;
using Hw1.Domain;
using Hw1.Application;

public class ShowReportCommandTests
{
    [Fact]
    public void ShowReportCommand_Execute_CallsReportGenerate()
    {
        // Arrange
        var serviceCenter = Substitute.For<IServiceCenter>();
        var park = new Park(serviceCenter);
        
        var report = Substitute.For<IReport>();
        var command = new ShowReportCommand();

        // Act
        var result = command.Execute(park, report);

        // Assert
        Assert.True(result);
        report.Received(1).GenerateReport(Arg.Any<ReportData>());
    }
}