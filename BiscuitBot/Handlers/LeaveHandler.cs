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
	ILogger<LeaveHandler> logger) : IGuildUserRemoveGatewayHandler
{
	public async ValueTask HandleAsync(
		GuildUserRemoveEventArgs args)
	{
		if (!configService.Config.LeaveEnabled)
			return;

		if (!configService.Config.LeaveChannelId.HasValue)
		{
			logger.LogWarning("Leave channel has not been set");
			return;
		}

		try
		{
			logger.LogInformation("Sending leave message for user {UserId} in guild {GuildId}", args.User.Id, args.GuildId);

			EmbedProperties embed = new()
			{
				Title = null,
				Description = $"**🖤 Goodbyee {args.User.Username}! 🖤**",
				Color = new(0, 0, 0),
				Fields =
				[
					new() { Name = "Account Created:", Value = $"<t:{args.User.CreatedAt.ToUnixTimeSeconds()}:D>" },
					new() { Name = "Account Joined:", Value = "Unknown" },
					new() { Name = "Invited By:", Value = "Unknown" },
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

			await restClient.SendMessageAsync(configService.Config.LeaveChannelId.Value, message);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to send leave message to channel {ChannelId}", configService.Config.LeaveChannelId.Value);
		}
	}
}
