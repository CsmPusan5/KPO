namespace Hw1.Application;

public class AddItemCommand(IInventoryItem item) : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        var result = park.AddItem(item);

        var message = result switch
        {
            AddResult.Accepted                => $"Принято: {item.GetInventoryNumber()}",
            AddResult.Rejected                => $"Техосмотр не пройден: {item.GetInventoryNumber()}",
            AddResult.DuplicateNumber         => $"Дубликат номера: {item.GetInventoryNumber()}",
            _                                 => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };

        Console.WriteLine(message);
        return true;
    }
}
