using Microsoft.Extensions.DependencyInjection;
using Hw1.Domain;
using Hw1.Application;

namespace Hw1;

public class Program {
    public static void Main(string[] args) {
        var services = new ServiceCollection();
        services.AddTransient<IServiceCenter, ServiceCenter>();
        services.AddTransient<IReport, Report>();
        services.AddTransient<IParser, Parser>();
        services.AddSingleton(sp => new Park(sp.GetRequiredService<IServiceCenter>()));
        
        var provider = services.BuildServiceProvider();

        var park = provider.GetRequiredService<Park>();
        var parser = provider.GetRequiredService<IParser>();
        var report = provider.GetRequiredService<IReport>();

        Console.WriteLine("ВышКат. Введите 'help' для списка команд.");
        while (true) {
            Console.Write("> ");
            string? input = Console.ReadLine();
            if (input is null) break;
            try
            {
                ICommand cmd = parser.Parse(input);
                if (!cmd.Execute(park, report)) break;
            }
            catch (ParseException e)
            {
                Console.WriteLine($"Ошибка: {e}");
            }
        }
    }
}