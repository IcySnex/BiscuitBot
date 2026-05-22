using BiscuitBot.Models;
using BiscuitBot.Services;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace BiscuitBot.Handlers;

public class LeaveHandler(
	ConfigService configService,
	RestClient restClient,
	InviteService inviteService,
	ILogger<LeaveHandler> logger) : IGuildUserRemoveGatewayHandler
{
	public async ValueTask HandleAsync(
		GuildUserRemoveEventArgs args)
	{
		GuildConfig config = configService.GetConfig(args.GuildId);
		if (!config.LeaveEnabled)
			return;

		if (!config.LeaveChannelId.HasValue)
		{
			logger.LogWarning("Leave channel has not been set");
			return;
		}

		try
		{
			logger.LogInformation("Sending leave message for user {UserId} in guild {GuildId}", args.User.Id, args.GuildId);

			MemberData? memberData = inviteService.GetMemberData(args.GuildId, args.User.Id);

			EmbedProperties embed = new()
			{
				Title = null,
				Description = $"**🖤 Goodbyee {args.User.Username}! 🖤**",
				Color = new(0, 0, 0),
				Fields =
				[
					new() { Name = "Account Created:", Value = $"<t:{args.User.CreatedAt.ToUnixTimeSeconds()}:f>" },
					new() { Name = "Account Joined:", Value = memberData is not null ? $"<t:{memberData.JoinedAt.ToUnixTimeSeconds()}:f>" : "Unknown" },
					new() { Name = "Invited By:", Value = memberData is not null ? memberData.InviterIsBot ? memberData.InviterName : $"<@{memberData.InviterId}>" : "Unknown" },
				],
				Footer = new()
				{
					Text = "🐺 We still hope you had a great time! 🐺"
				},
				Thumbnail = new(args.User.GetAvatarUrl()?.ToString() ?? args.User.DefaultAvatarUrl.ToString())
			};

			MessageProperties message = new()
			{
				Embeds = [embed]
			};

			await restClient.SendMessageAsync(config.LeaveChannelId.Value, message);
			
			inviteService.ForgetMember(args.GuildId, args.User.Id);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to send leave message to channel {ChannelId}", config.LeaveChannelId.Value);
		}
	}
}
