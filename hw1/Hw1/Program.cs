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
        services.AddTransient<App>();

        var provider = services.BuildServiceProvider();

        var app = provider.GetRequiredService<App>();

        app.Run();
    }
}