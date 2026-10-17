using Hw1.Domain;

namespace Hw1.Application;

public class HelpCommand : ICommand
{
    public bool Execute(Park park, IReport report)
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("  add scooter <number> <name> <easyScore> <energyKwh>");
        Console.WriteLine("  add bike    <number> <name> <easyScore>");
        Console.WriteLine("  add ebike   <number> <name> <easyScore> <energyKwh>");
        Console.WriteLine("  add helmet  <number> <name> <easyScore>");
        Console.WriteLine("  add charger <number> <name> <easyScore> <energyKwh>");
        Console.WriteLine("  report  - отчёт: количество, энергия, весь инвентарь");
        Console.WriteLine("  easy    - техника, подходящая новичкам");
        Console.WriteLine("  list    - вся теника в парке");
        Console.WriteLine("  help    - эта справка");
        Console.WriteLine("  exit    - выход");
        return true;
    }
}