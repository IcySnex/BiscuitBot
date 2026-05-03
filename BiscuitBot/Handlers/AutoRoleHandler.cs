using BiscuitBot.Models;
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
		GuildConfig config = configService.GetConfig(user.GuildId);
		if (!config.AutoRoleEnabled || !config.AutoRoleRoleId.HasValue)
			return;

		try
		{
			logger.LogInformation("Applying AutoRole {RoleId} to user {UserId} in guild {GuildId}", config.AutoRoleRoleId.Value, user.Id, user.GuildId);
			await user.AddRoleAsync(config.AutoRoleRoleId.Value);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to apply AutoRole to user {UserId}", user.Id);
		}
	}
}
