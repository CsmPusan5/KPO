public class ShowEasyCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine("Легкие вещи:");
        foreach (var item in park.GetEasyForBeginers())
        {
            Console.WriteLine($"    {item.GetType()}: {item.GetName()}, Номер: {item.GetInventoryNumber()}");
        }
        return true;
    }
}