using Cli.Commands;
using CliFx;
using Lib.Interfaces;
using Lib.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddSingleton<IPafIO, PafIo>();
services.AddSingleton<IFastaIO, FastaIO>();
services.AddSingleton<IConsensusBuilder, ConsensusBuilder>();
services.AddSingleton<ISequenceBuilder, SequenceBuilder>();
services.AddSingleton<IGraphExtender, GraphExtender>();

services.AddTransient<ImproveReads>();

var serviceProvider = services.BuildServiceProvider();

return await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .UseTypeActivator(serviceProvider)
    .Build()
    .RunAsync();