using BiscuitBot.Models;
using BiscuitBot.Services;
using BiscuitBot.Utils;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace BiscuitBot.Handlers;

public class WelcomeHandler(
	ConfigService configService,
	RestClient restClient,
	GatewayClient gatewayClient,
	InviteService inviteService,
	ILogger<WelcomeHandler> logger) : IGuildUserAddGatewayHandler
{
	public async ValueTask HandleAsync(
		GuildUser user)
	{
		GuildConfig config = configService.GetConfig(user.GuildId);
		if (!config.WelcomeEnabled)
			return;
		
		if (!config.WelcomeChannelId.HasValue)
		{
			logger.LogWarning("Welcome channel has not been set");
			return;
		}

		try
		{
			logger.LogInformation("Sending welcome message for user {UserId} in guild {GuildId}", user.Id, user.GuildId);

			int memberCount = 0;
			if (gatewayClient.Cache.Guilds.TryGetValue(user.GuildId, out Guild? guild))
				memberCount = guild.Users.Count;

			User? inviter = await inviteService.GetInviterAsync(user.GuildId);

			if (user.JoinedAt.HasValue)
				inviteService.TrackMember(user.GuildId, user.Id, inviter?.IsBot ?? false, inviter?.Id, inviter?.Username, user.JoinedAt.Value);

			EmbedProperties embed = new()
			{
				Title = null,
				Description = $"**🌸 Welcome {user}! 🌸**",
				Color = new(190, 173, 255),
				Fields =
				[
					new() { Name = "Account Created:", Value = $"<t:{user.CreatedAt.ToUnixTimeSeconds()}:f>" },
					new() { Name = "Account Joined:", Value = user.JoinedAt.HasValue ? $"<t:{user.JoinedAt.Value.ToUnixTimeSeconds()}:f>" : "Unknown" },
					new() { Name = "Invited By:", Value = inviter is not null ? inviter.IsBot ? inviter.Username : inviter.ToString() : "Unknown" },
				],
				Footer = new()
				{
					Text = $"🌺 You are the {FormatUtils.Ordinal(memberCount)} member! 🌺"
				},
				Thumbnail = new(user.GetAvatarUrl()?.ToString() ?? user.DefaultAvatarUrl.ToString())
			};

			MessageProperties message = new()
			{
				Embeds = [embed]
			};

			await restClient.SendMessageAsync(config.WelcomeChannelId.Value, message);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to send welcome message to channel {ChannelId}", config.WelcomeChannelId.Value);
		}
	}
}
