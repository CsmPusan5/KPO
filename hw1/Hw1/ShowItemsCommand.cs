public class ShowItemsCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine("Вещи:");
        foreach (var thing in park.GetItems())
        {
            Console.WriteLine($"    {thing.GetType()}: {thing.GetName()}, Номер: {thing.GetInventoryNumber()}");
        }
        return true;
    }
}