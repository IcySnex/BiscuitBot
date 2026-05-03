namespace BiscuitBot.Models;

public class GuildConfig
{
	public bool AutoRoleEnabled { get; set; }
	
	public ulong? AutoRoleRoleId { get; set; }


	
	public bool WelcomeEnabled { get; set; }
	
	public ulong? WelcomeChannelId { get; set; }


	public bool LeaveEnabled { get; set; }

	public ulong? LeaveChannelId { get; set; }
}
