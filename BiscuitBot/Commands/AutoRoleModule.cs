using NetCord;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

[SlashCommand("auto-role", "AutoRole commands")]
public class AutoRoleModule : ApplicationCommandModule<ApplicationCommandContext>
{
	[SubSlashCommand("set", "Sets the auto-role")]
	public string Set(
		Role role)
	{
		return $"AutoRole set to: <@&{role.Id}>";
	}
}
