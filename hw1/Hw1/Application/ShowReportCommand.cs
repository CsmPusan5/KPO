using Hw1.Domain;

namespace Hw1.Application;

public class ShowReportCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        var items = park.GetItems();

        var data = new ReportData();
        data.SetTotalItems(items.Count);
        data.SetTotalEnergy(park.GetEnergyUsage());
        data.SetTransport([.. items.OfType<Transport>()]);
        data.SetThings([.. items.OfType<Thing>()]);
        Console.WriteLine(report.GenerateReport(data));
        return true;
    }
}