using System.Text.Json;

namespace BiscuitBot.Utils;

public static class JsonConverter
{
	static readonly JsonSerializerOptions FormattedOptions = new()
	{
		WriteIndented = true
	};

	public static string AsString<T>(
		T obj,
		bool formatted = false) =>
		JsonSerializer.Serialize(obj, formatted ? FormattedOptions : null);

	public static T As<T>(
		string json) =>
		JsonSerializer.Deserialize<T>(json) ?? throw new JsonException("Failed to deserialize JSON.");
}