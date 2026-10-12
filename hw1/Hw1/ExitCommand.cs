public class ExitCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine("Выход");
        return false;
    }
}