using BiscuitBot.Models;
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
		configService.GetConfig(Context.Guild!.Id).LeaveEnabled = true;
		configService.Save();

		logger.LogInformation("Leave feature enabled");
		return "Leave feature has been enabled.";
	}

	[SubSlashCommand("disable", "Disables the leave feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Disable()
	{
		configService.GetConfig(Context.Guild!.Id).LeaveEnabled = false;
		configService.Save();

		logger.LogInformation("Leave feature disabled");
		return "Leave feature has been disabled.";
	}

	
	[SubSlashCommand("status", "Shows the current leave status")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Status()
	{
		GuildConfig config = configService.GetConfig(Context.Guild!.Id);
		string status = config.LeaveEnabled ? "Enabled" : "Disabled";
		string channel = config.LeaveChannelId.HasValue ? $"<#{config.LeaveChannelId}>" : "None";

		return $"Leave Status: {status}\nChannel: {channel}";
	}

	
	[SubSlashCommand("set", "Sets the leave channel")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Set(
		TextGuildChannel channel)
	{
		configService.GetConfig(Context.Guild!.Id).LeaveChannelId = channel.Id;
		configService.Save();

		logger.LogInformation("Leave channel set to: {ChannelName} ({ChannelId})", channel.Name, channel.Id);
		return $"Leave channel set to: {channel}";
	}
}
