
public class Park(IServiceCenter serviceCenter)
{
    private readonly List<IInventoryItem> _items = [];
    private readonly IServiceCenter _serviceCenter = serviceCenter;

    public AddResult AddItem(IInventoryItem item)
    {
        if (_items.Any(x => x.GetInventoryNumber() == item.GetInventoryNumber()))
            return AddResult.DuplicateNumber;

        if (!_serviceCenter.Check(item))
            Console.WriteLine("Проверка не пройдена");

        _items.Add(item);
        Console.WriteLine($"Добавлено: {item.GetName()}");
        return AddResult.Accepted; 
    }
    public double GetEnergyUsage()
    {
        double totalUsage = 0;
        foreach (var item in _items.OfType<IEnergyConsumer>())
        {
            totalUsage += item.GetEnergyUsage();
        }
        return totalUsage;
    }
    public List<InventoryItem> GetEasyForBeginers() =>
        [.. _items.OfType<InventoryItem>().Where(x => x.IsEasyForBeginners())];

    public List<IInventoryItem> GetItems() => [.. _items];
    public ReportData GetData()
    {
        ReportData data = new();
        data.SetTotalItems(_items.Count);
        data.SetTotalEnergy(GetEnergyUsage());
        data.SetTransport([.. _items.OfType<Transport>()]);
        data.SetThings([.. _items.OfType<Thing>()]);
        return data;
    }
    
}