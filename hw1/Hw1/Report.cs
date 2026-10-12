public class Report : IReport
{
    private string report = "";
    public string GenerateReport(ReportData data)
    {
        report = $"Отчет:\nВсего устройств: {data.GetTotalItems()}\nВсего энергии используется: {data.GetTotalEnergy()}\n";
        report += "Транспорт:\n";
        foreach (var transport in data.GetTransport())
        {
            report += $"    Имя: {transport.GetName()}, Номер: {transport.GetInventoryNumber()}\n";
        }
        report += "Вещи:\n";
        foreach (var thing in data.GetThings())
        {
            report += $"    Имя: {thing.GetName()}, Номер: {thing.GetInventoryNumber()}\n";
        }
        return report;
    }
}