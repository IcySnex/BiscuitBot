using BiscuitBot.Services;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace BiscuitBot.Handlers;

public class InviteHandler(
	InviteService inviteService) : 
	IGuildCreateGatewayHandler, 
	IInviteCreateGatewayHandler, 
	IInviteDeleteGatewayHandler
{
	public async ValueTask HandleAsync(
		GuildCreateEventArgs args)
	{
		await inviteService.InitializeAsync(args.GuildId);
	}

	public ValueTask HandleAsync(
		Invite invite)
	{
		if (invite.GuildId.HasValue)
			inviteService.AddInvite(invite.GuildId.Value, invite.Code, invite.Uses);
		
		return ValueTask.CompletedTask;
	}

	public ValueTask HandleAsync(
		InviteDeleteEventArgs args)
	{
		if (args.GuildId.HasValue)
			inviteService.RemoveInvite(args.GuildId.Value, args.InviteCode);
		
		return ValueTask.CompletedTask;
	}
}
