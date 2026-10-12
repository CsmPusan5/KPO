public class ShowItemsCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine($"Вещи: {string.Join(", ", park.GetItems())}");
        return true;
    }
}