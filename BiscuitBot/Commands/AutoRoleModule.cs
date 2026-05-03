using BiscuitBot.Services;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

[SlashCommand("auto-role", "AutoRole commands", DefaultGuildPermissions = Permissions.Administrator)]
public class AutoRoleModule(
	ConfigService configService,
	ILogger<AutoRoleModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
	[SubSlashCommand("enable", "Enables the auto-role feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Enable()
	{
		configService.Config.AutoRoleEnabled = true;
		configService.Save();

		logger.LogInformation("AutoRole feature enabled");
		return "AutoRole feature has been enabled.";
	}

	[SubSlashCommand("disable", "Disables the auto-role feature")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Disable()
	{
		configService.Config.AutoRoleEnabled = false;
		configService.Save();

		logger.LogInformation("AutoRole feature disabled");
		return "AutoRole feature has been disabled.";
	}

	
	[SubSlashCommand("status", "Shows the current auto-role status")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Status()
	{
		string status = configService.Config.AutoRoleEnabled ? "Enabled" : "Disabled";
		string role = configService.Config.AutoRoleId.HasValue ? $"<@&{configService.Config.AutoRoleId}>" : "None";

		return $"AutoRole Status: {status}\nRole: {role}";
	}
	
	
	[SubSlashCommand("set", "Sets the auto-role")]
	[RequireUserPermissions<ApplicationCommandContext>(Permissions.Administrator)]
	public string Set(
		Role role)
	{
		if (role.Id == Context.Guild!.Id)
			return "You cannot set the '@everyone' role as an auto-role.";

		if (role.Managed)
			return "You cannot set a managed role (e.g., bot or integration role) as an auto-role.";

		configService.Config.AutoRoleId = role.Id;
		configService.Save();

		logger.LogInformation("AutoRole set to: {RoleName} ({RoleId})", role.Name, role.Id);
		return $"AutoRole set to: {role}";
	}
}
