using BiscuitBot.Services;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace BiscuitBot.Handlers;

public class WelcomeHandler(
	ConfigService configService,
	RestClient restClient,
	ILogger<WelcomeHandler> logger) : IGuildUserAddGatewayHandler
{
	public async ValueTask HandleAsync(
		GuildUser user)
	{
		if (!configService.Config.WelcomeEnabled)
			return;
		
		if (!configService.Config.WelcomeChannelId.HasValue)
		{
			logger.LogWarning("Welcome channel has not been set");
			return;
		}

		try
		{
			logger.LogInformation("Sending welcome message for user {UserId} in guild {GuildId}", user.Id, user.GuildId);

			EmbedProperties embed = new()
			{
				Title = $"Welcome to the server, {user.Username}!",
				Description = "We are glad to have you here! Make sure to read the rules and enjoy your stay.",
				Color = new(0, 255, 0),
				Thumbnail = new(user.GetAvatarUrl()?.ToString()),
				Footer = new()
				{
					Text = $"User ID: {user.Id}"
				},
				Timestamp = DateTimeOffset.UtcNow
			};

			MessageProperties message = new()
			{
				Embeds = [embed]
			};

			await restClient.SendMessageAsync(configService.Config.WelcomeChannelId.Value, message);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to send welcome message to channel {ChannelId}", configService.Config.WelcomeChannelId.Value);
		}
	}
}
