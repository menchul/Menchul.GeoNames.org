using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Menchul.Import.GeoNames.org.IntegrationTests;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.UTF8;
        Console.ResetColor();

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddNLog());
        services.AddTransient<IntegrationTestOrchestrator>();

        await using ServiceProvider serviceProvider = services.BuildServiceProvider();
        ILogger<IntegrationTestOrchestrator> logger = serviceProvider.GetRequiredService<ILogger<IntegrationTestOrchestrator>>();
        var orchestrator = new IntegrationTestOrchestrator(logger);

        bool success = await orchestrator.RunAllAsync(true);

        if (success)
        {
            return 0;
        }

        return 1;
    }
}