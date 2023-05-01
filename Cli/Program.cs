using Cli.Commands;
using CliFx;
using Lib.Interfaces;
using Lib.Services;
using Microsoft.Extensions.DependencyInjection;

public static class Program
{
    public static async Task<int> Main()
    {
        var services = new ServiceCollection();

        // register services
        services.AddSingleton<IRandomSequenceGenerator, RandomSequenceGenerator>();
        services.AddSingleton<ISequenceLoader, SequenceLoader>();

        // register commands
        services.AddTransient<ReadFastaCommand>();
        services.AddTransient<GenerateRandom>();

        var serviceProvider = services.BuildServiceProvider();

        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(serviceProvider)
            .Build()
            .RunAsync();
    }
}