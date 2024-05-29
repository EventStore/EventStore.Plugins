namespace EventStore.Plugins.Diagnostics;

/// <summary>
///     Represents diagnostic data of a plugin.
///     By default it is a snapshot and will override previously collected data, by event name.
/// </summary>
public readonly record struct PluginDiagnosticsData() : IComparable<PluginDiagnosticsData>, IComparable {
	/// <summary>
	///		The source of the event that matches the DiagnosticsName.
	/// </summary>
	public required string Source { get; init; }
	
	/// <summary>
	///		The name of the event. The default is PluginDiagnosticsData.
	/// </summary>
	public string EventName { get; init; } = nameof(PluginDiagnosticsData);
	
	/// <summary>
	///		The data associated with the event in the form of a dictionary.
	/// </summary>
	public required Dictionary<string, object?> Data { get; init; }
	
	/// <summary>
	///		When the event occurred.
	/// </summary>
	public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
	
	/// <summary>
	///		Whether the event is a snapshot and should override previously collected data, by event name. Default value is true.
	/// </summary>
	public bool IsSnapshot { get; init; } = false;
	
	/// <summary>
	///		Gets the value associated with the specified key.
	/// </summary>
	public T GetValue<T>(string key, T defaultValue) => 
		Data.TryGetValue(key, out var value) &&
		value is T typedValue ? typedValue : defaultValue;
	
	/// <summary>
	///		Gets the value associated with the specified key.
	/// </summary>
	public T? GetValue<T>(string key) => 
		Data.TryGetValue(key, out var value) ? (T?)value : default;
	
	public int CompareTo(PluginDiagnosticsData other) {
		var sourceComparison = string.Compare(Source, other.Source, StringComparison.Ordinal);
		if (sourceComparison != 0) return sourceComparison;

		var eventNameComparison = string.Compare(EventName, other.EventName, StringComparison.Ordinal);
		if (eventNameComparison != 0) return eventNameComparison;

		return Timestamp.CompareTo(other.Timestamp);
	}

	public int CompareTo(object? obj) {
		if (ReferenceEquals(null, obj)) return 1;

		return obj is PluginDiagnosticsData other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(PluginDiagnosticsData)}");
	}

	public static bool operator <(PluginDiagnosticsData left, PluginDiagnosticsData right) => left.CompareTo(right) < 0;

	public static bool operator >(PluginDiagnosticsData left, PluginDiagnosticsData right) => left.CompareTo(right) > 0;

	public static bool operator <=(PluginDiagnosticsData left, PluginDiagnosticsData right) => left.CompareTo(right) <= 0;

	public static bool operator >=(PluginDiagnosticsData left, PluginDiagnosticsData right) => left.CompareTo(right) >= 0;
}
