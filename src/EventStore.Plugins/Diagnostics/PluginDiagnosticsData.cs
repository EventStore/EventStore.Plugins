namespace EventStore.Plugins.Diagnostics;

/// <summary>
///     Represents diagnostic data of a plugin.
/// </summary>
/// <param name="Source">The source of the event that matches the DiagnosticsName</param>
/// <param name="EventName">The name of the event. The default is PluginDiagnosticsData.</param>
/// <param name="Data">The data associated with the event in the form of a dictionary.</param>
/// <param name="Timestamp">When the event occurred.</param>
/// <param name="IsSnapshot">Whether the event is a snapshot and should override previously collected data, by event name. Default value is true.</param>
public readonly record struct PluginDiagnosticsData(
	string Source,
	string EventName,
	Dictionary<string, object?> Data,
	DateTimeOffset Timestamp,
	bool IsSnapshot = true
);