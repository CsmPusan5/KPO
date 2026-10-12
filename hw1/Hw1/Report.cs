public class Report : IReport
{
    private string report = "";
    public string GenerateReport(ReportData data)
    {
        report = $"Отчет:\nВсего устройств: {data.GetTotalItems()}\nВсего энергии используется: {data.GetTotalEnergy()}\n";
        report += "Транспорт:\n";
        foreach (var transport in data.GetTransport())
        {
            report += $"    {transport.GetType()}: {transport.GetName()}, Номер: {transport.GetInventoryNumber()}\n";
        }
        report += "Вещи:\n";
        foreach (var thing in data.GetThings())
        {
            report += $"    {thing.GetType()}: {thing.GetName()}, Номер: {thing.GetInventoryNumber()}\n";
        }
        return report;
    }
}