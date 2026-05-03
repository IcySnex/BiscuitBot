using Serilog.Core;
using Serilog.Events;

namespace BiscuitBot.Utils;

public class SourceContextEnricher : ILogEventEnricher
{
	public void Enrich(
		LogEvent logEvent,
		ILogEventPropertyFactory propertyFactory)
	{
		if (!logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue? value))
			return;

		string fullName = value.ToString()?.Trim('"') ?? "";
		string className = fullName.Contains('.')
			? fullName[(fullName.LastIndexOf('.') + 1)..]
			: fullName;

		logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("Class", className));
	}
}