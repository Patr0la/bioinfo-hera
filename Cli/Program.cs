using Cli.Commands;
using CliFx;
using Lib.Entities;
using Lib.Interfaces;
using Lib.Services;
using Microsoft.Extensions.DependencyInjection;
using QuikGraph;

public static class Program
{
    public static async Task<int> Main()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IFileLoader, FileLoader>();
        
        services.AddTransient<ReadFastaCommand>();
        
        var serviceProvider = services.BuildServiceProvider();

        return await new CliApplicationBuilder()
            .AddCommandsFromThisAssembly()
            .UseTypeActivator(serviceProvider)
            .Build()
            .RunAsync();
    }
}