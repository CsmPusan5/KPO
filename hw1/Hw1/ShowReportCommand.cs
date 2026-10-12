public class ShowReportCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine(report.GenerateReport(park.GetData()));
        return true;
    }
}