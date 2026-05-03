using BiscuitBot.Services;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

[SlashCommand("leave", "Leave commands", DefaultGuildPermissions = Permissions.Administrator)]
public class LeaveModule(
	ConfigService configService,
	ILogger<LeaveModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
	[SubSlashCommand("enable", "Enables the leave feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Enable()
	{
		configService.Config.LeaveEnabled = true;
		configService.Save();

		logger.LogInformation("Leave feature enabled");
		return "Leave feature has been enabled.";
	}

	[SubSlashCommand("disable", "Disables the leave feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Disable()
	{
		configService.Config.LeaveEnabled = false;
		configService.Save();

		logger.LogInformation("Leave feature disabled");
		return "Leave feature has been disabled.";
	}

	
	[SubSlashCommand("status", "Shows the current leave status")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Status()
	{
		string status = configService.Config.LeaveEnabled ? "Enabled" : "Disabled";
		string channel = configService.Config.LeaveChannelId.HasValue ? $"<#{configService.Config.LeaveChannelId}>" : "None";

		return $"Leave Status: {status}\nChannel: {channel}";
	}

	
	[SubSlashCommand("set", "Sets the leave channel")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Set(
		TextGuildChannel channel)
	{
		configService.Config.LeaveChannelId = channel.Id;
		configService.Save();

		logger.LogInformation("Leave channel set to: {ChannelName} ({ChannelId})", channel.Name, channel.Id);
		return $"Leave channel set to: {channel}";
	}
}
