using System.Collections.Concurrent;

namespace EventStore.Plugins.Diagnostics;

/// <summary>
///     A delegate to handle <see cref="PluginDiagnosticsData" /> events.
/// </summary>
public delegate void OnEventCollected(PluginDiagnosticsData diagnosticsData);

/// <summary>
///     Component to collect diagnostics data from plugins. More specifically <see cref="PluginDiagnosticsData" /> events.
/// </summary>
[PublicAPI]
public class PluginDiagnosticsDataCollector : IDisposable {
	public PluginDiagnosticsDataCollector(string[] sources, int capacity = 10, OnEventCollected? onEventCollected = null) {
		Listener = new(sources, 0, (source, data) => {
			if (data is not PluginDiagnosticsData pluginData) return;
			
			CollectedEventsByPlugin.AddOrUpdate(
				source,
				static (_, state) => [state.PluginData],
				static (_, collected, state) => {
					if (state.PluginData.IsSnapshot)
						collected.RemoveWhere(x => x.EventName == state.PluginData.EventName);

					collected.Add(state.PluginData);

					if (collected.Count > state.Capacity)
						collected.Remove(collected.Min);
					
					return collected;
				},
				(PluginData: pluginData, Capacity: capacity)
			);

			try {
				onEventCollected?.Invoke(pluginData);
			}
			catch (Exception) {
				// stay on target
			}
		});
	}

	MultiSourceDiagnosticsListener Listener { get; }
	
	ConcurrentDictionary<string, SortedSet<PluginDiagnosticsData>> CollectedEventsByPlugin { get; } = new();

	public IEnumerable<PluginDiagnosticsData> CollectedEvents(string source) =>
		CollectedEventsByPlugin.TryGetValue(source, out var data) ? data : Array.Empty<PluginDiagnosticsData>();

	public bool HasCollectedEvents(string source) =>
		CollectedEventsByPlugin.TryGetValue(source, out var data) && data.Count > 0;

	public void ClearCollectedEvents(string source) {
		if (CollectedEventsByPlugin.TryGetValue(source, out var data))
			data.Clear();
	}
	
	public void ClearAllCollectedEvents() {
		foreach (var data in CollectedEventsByPlugin.Values)
			data.Clear();
	}

	public void Dispose() {
		Listener.Dispose();
		CollectedEventsByPlugin.Clear();
	}
	
	/// <summary>
	///     Starts the <see cref="PluginDiagnosticsDataCollector" /> with the specified delegate and sources.
	///     This method is a convenient way to create a new instance of the <see cref="PluginDiagnosticsDataCollector" /> and start collecting data immediately.
	/// </summary>
	/// <param name="onEventCollected">A delegate to handle <see cref="PluginDiagnosticsData" /> events.</param>
	/// <param name="sources">The plugin diagnostic names to collect diagnostics data from.</param>
	public static PluginDiagnosticsDataCollector Start(OnEventCollected onEventCollected, params string[] sources) =>
			new(sources, 10, onEventCollected);
	
	/// <summary>
	///     Starts the <see cref="PluginDiagnosticsDataCollector" /> with the specified delegate and sources.
	///     This method is a convenient way to create a new instance of the <see cref="PluginDiagnosticsDataCollector" /> and start collecting data immediately.
	/// </summary>
	/// <param name="onEventCollected">A delegate to handle <see cref="PluginDiagnosticsData" /> events.</param>
	/// <param name="capacity">The maximum number of diagnostics data to collect per source.</param>
	/// <param name="sources">The plugin diagnostic names to collect diagnostics data from.</param>
	public static PluginDiagnosticsDataCollector Start(OnEventCollected onEventCollected, int capacity, params string[] sources) =>
		new(sources, capacity, onEventCollected);

	/// <summary>
	///     Starts the <see cref="PluginDiagnosticsDataCollector" /> with the specified sources.
	///     This method is a convenient way to create a new instance of the <see cref="PluginDiagnosticsDataCollector" /> and start collecting data immediately.
	/// </summary>
	/// <param name="sources">
	///     The plugin diagnostic names to collect diagnostics data from.
	/// </param>
	public static PluginDiagnosticsDataCollector Start(params string[] sources) => new(sources);

	/// <summary>
	///     Starts the <see cref="PluginDiagnosticsDataCollector" /> with the specified sources.
	///     This method is a convenient way to create a new instance of the <see cref="PluginDiagnosticsDataCollector" /> and start collecting data immediately.
	/// </summary>
	/// <param name="capacity">The maximum number of diagnostics data to collect per source.</param>
	/// <param name="sources">The plugin diagnostic names to collect diagnostics data from.</param>
	public static PluginDiagnosticsDataCollector Start(int capacity, params string[] sources) => new(sources, capacity);
}
