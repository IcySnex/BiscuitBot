using System.Reflection;
using BiscuitBot.Handlers;
using BiscuitBot.Services;
using BiscuitBot.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Serilog;

namespace BiscuitBot;

public class Program
{
	public static async Task Main(
		string[] args)
	{
		const string loggerTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {Class}] {Message:lj}{NewLine}{Exception}";
		Log.Logger = new LoggerConfiguration()
			.Enrich.With<SourceContextEnricher>()
			.WriteTo.File(
				"Logs/log-.txt",
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 5,
				outputTemplate: loggerTemplate)
			.WriteTo.Console(
				outputTemplate: loggerTemplate)
			.CreateLogger();

		try
		{
			Log.Information("Starting BiscuitBot...");

			HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

			builder.Services.AddSerilog();
			
			builder.Services.AddSingleton<ConfigService>();

			builder.Services.AddDiscordGateway(options =>
			{
				options.Intents = GatewayIntents.Guilds | GatewayIntents.GuildUsers;
			});
			builder.Services.AddApplicationCommands();

			builder.Services.AddGatewayHandler<AutoRoleHandler>();
			builder.Services.AddGatewayHandler<WelcomeHandler>();
			builder.Services.AddGatewayHandler<LeaveHandler>();

			IHost host = builder.Build();

			host.AddModules(Assembly.GetExecutingAssembly());

			await host.RunAsync();
		}
		catch (Exception exception)
		{
			Log.Fatal(exception, "Application terminated unexpectedly");
		}
		finally
		{
			await Log.CloseAndFlushAsync();
		}
	}
}
