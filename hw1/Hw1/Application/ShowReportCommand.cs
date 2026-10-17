using Hw1.Domain;

namespace Hw1.Application;

public class ShowReportCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        var data = new ReportData();
        data.SetTotalItems(park.GetItems().Count);
        data.SetTotalEnergy(park.GetEnergyUsage());
        data.SetTransport(park.GetTransport());
        data.SetThings(park.GetThings());
        Console.WriteLine(report.GenerateReport(data));
        return true;
    }
}