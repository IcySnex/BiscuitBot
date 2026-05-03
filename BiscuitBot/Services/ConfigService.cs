using BiscuitBot.Models;
using BiscuitBot.Utils;
using Microsoft.Extensions.Logging;

namespace BiscuitBot.Services;

public class ConfigService(
	ILogger<ConfigService> logger)
{
	const string ConfigPath = "config.json";
	

	public GuildConfig Config { get; } = Load();


	static GuildConfig Load()
	{
		if (!File.Exists(ConfigPath))
			return new();

		try
		{
			string json = File.ReadAllText(ConfigPath);
			return JsonConverter.As<GuildConfig>(json);
		}
		catch
		{
			return new();
		}
	}
	
	public void Save()
	{
		try
		{
			string json = JsonConverter.AsString(Config);
			File.WriteAllText(ConfigPath, json);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Failed to save configuration.");
		}
	}
}
