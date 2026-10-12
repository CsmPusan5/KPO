public class ShowEasyCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine($"Легкие вещи: {string.Join(", ", park.GetEasyForBeginers())}");
        return true;
    }
}