using Microsoft.Extensions.Logging;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

[SlashCommand("leave", "Leave commands")]
public class LeaveModule(
	ILogger<LeaveModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
	[SubSlashCommand("set", "Sets the leave message")]
	public string Set(
		string message)
	{
		logger.LogInformation("Leave message updated to: {Message}", message);
		return $"Leave message set to: {message}";
	}
}
