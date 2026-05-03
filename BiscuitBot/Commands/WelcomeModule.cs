using BiscuitBot.Services;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

[SlashCommand("welcome", "Welcome commands", DefaultGuildPermissions = Permissions.Administrator)]
public class WelcomeModule(
	ConfigService configService,
	ILogger<WelcomeModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
	[SubSlashCommand("enable", "Enables the welcome feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Enable()
	{
		configService.Config.WelcomeEnabled = true;
		configService.Save();

		logger.LogInformation("Welcome feature enabled");
		return "Welcome feature has been enabled.";
	}

	[SubSlashCommand("disable", "Disables the welcome feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Disable()
	{
		configService.Config.WelcomeEnabled = false;
		configService.Save();

		logger.LogInformation("Welcome feature disabled");
		return "Welcome feature has been disabled.";
	}

	
	[SubSlashCommand("status", "Shows the current welcome status")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Status()
	{
		string status = configService.Config.WelcomeEnabled ? "Enabled" : "Disabled";
		string channel = configService.Config.WelcomeChannelId.HasValue ? $"<#{configService.Config.WelcomeChannelId}>" : "None";

		return $"Welcome Status: {status}\nChannel: {channel}";
	}

	
	[SubSlashCommand("set", "Sets the welcome channel")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Set(
		TextGuildChannel channel)
	{
		configService.Config.WelcomeChannelId = channel.Id;
		configService.Save();

		logger.LogInformation("Welcome channel set to: {ChannelName} ({ChannelId})", channel.Name, channel.Id);
		return $"Welcome channel set to: {channel}";
	}
}
