using BiscuitBot.Services;
using NetCord.Hosting.Gateway;
using Microsoft.Extensions.Logging;
using NetCord;

namespace BiscuitBot.Handlers;

public class AutoRoleHandler(
	ConfigService configService,
	ILogger<AutoRoleHandler> logger) : IGuildUserAddGatewayHandler
{
	public async ValueTask HandleAsync(
		GuildUser user)
	{
		if (!configService.Config.AutoRoleEnabled || !configService.Config.AutoRoleId.HasValue)
			return;

		try
		{
			logger.LogInformation("Applying AutoRole {RoleId} to user {UserId} in guild {GuildId}", configService.Config.AutoRoleId.Value, user.Id, user.GuildId);
			await user.AddRoleAsync(configService.Config.AutoRoleId.Value);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to apply AutoRole to user {UserId}", user.Id);
		}
	}
}
