using Microsoft.Extensions.Logging;
using NetCord.Services.ApplicationCommands;

namespace BiscuitBot.Commands;

[SlashCommand("welcome", "Welcome commands")]
public class WelcomeModule(
	ILogger<WelcomeModule> logger) : ApplicationCommandModule<ApplicationCommandContext>
{
	[SubSlashCommand("set", "Sets the welcome message")]
	public string Set(
		string message)
	{
		logger.LogInformation("Welcome message updated to: {Message}", message);
		return $"Welcome message set to: {message}";
	}
}
