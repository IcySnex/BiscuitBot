using BiscuitBot.Services;
using Microsoft.Extensions.Logging;
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
				Title = "Goodbye!",
				Description = $"{args.User.Username} has left the server. We'll miss you!",
				Color = new(255, 0, 0),
				Thumbnail = new(args.User.GetAvatarUrl()?.ToString()),
				Footer = new()
				{
					Text = $"User ID: {args.User.Id}"
				},
				Timestamp = DateTimeOffset.UtcNow
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
