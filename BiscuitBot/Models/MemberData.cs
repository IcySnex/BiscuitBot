namespace BiscuitBot.Models;

public class MemberData
{
	public bool InviterIsBot { get; set; }
	public ulong? InviterId { get; set; }
	public string? InviterName { get; set; }
	public DateTimeOffset JoinedAt { get; set; }
}