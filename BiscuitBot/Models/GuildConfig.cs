namespace BiscuitBot.Models;

public class GuildConfig
{
	public ulong? AutoRoleId { get; set; }

	public bool AutoRoleEnabled { get; set; }

	public ulong? WelcomeChannelId { get; set; }

	public bool WelcomeEnabled { get; set; }
}
