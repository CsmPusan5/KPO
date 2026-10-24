using Hw1.Domain;

namespace Hw1.Application;

public class ShowEasyCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine("Легкие вещи:");
        foreach (var item in park.GetEasyForBeginners())
        {
            Console.WriteLine($"    {item.GetType()}: {item.GetName()}, Номер: {item.GetInventoryNumber()}");
        }
        return true;
    }
}