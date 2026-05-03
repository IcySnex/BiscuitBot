using System.Collections.Concurrent;
using System.Text.Json;
using BiscuitBot.Models;
using BiscuitBot.Utils;
using Microsoft.Extensions.Logging;

namespace BiscuitBot.Services;

public class ConfigService(
	ILogger<ConfigService> logger)
{
	const string ConfigPath = "config.json";
	

	readonly ConcurrentDictionary<ulong, GuildConfig> configs = Load(logger);


	static ConcurrentDictionary<ulong, GuildConfig> Load(
		ILogger logger)
	{
		if (!File.Exists(ConfigPath))
			return [];

		try
		{
			logger.LogInformation("Reading config from {Path}", ConfigPath);
			string json = File.ReadAllText(ConfigPath);
			Dictionary<string, GuildConfig>? rawData = JsonSerializer.Deserialize<Dictionary<string, GuildConfig>>(json);
			
			if (rawData is null)
				return [];

			ConcurrentDictionary<ulong, GuildConfig> result = [];
			foreach (KeyValuePair<string, GuildConfig> pair in rawData)
			{
				if (ulong.TryParse(pair.Key, out ulong guildId))
					result[guildId] = pair.Value;
			}
			return result;
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to load configuration.");
			return [];
		}
	}
	
	public GuildConfig GetConfig(
		ulong guildId)
	{
		return configs.GetOrAdd(
			guildId,
			_ => new());
	}

	public void Save()
	{
		try
		{
			logger.LogInformation("Saving config to {Path}", ConfigPath);
			
			Dictionary<string, GuildConfig> rawData = [];
			foreach (KeyValuePair<ulong, GuildConfig> pair in configs)
				rawData[pair.Key.ToString()] = pair.Value;

			string json = JsonSerializer.Serialize(rawData);
			File.WriteAllText(ConfigPath, json);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to save configuration.");
		}
	}
}
