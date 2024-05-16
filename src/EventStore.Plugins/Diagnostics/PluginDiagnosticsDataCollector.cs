using System.Collections.Concurrent;
using System.Diagnostics;

namespace EventStore.Plugins.Diagnostics;

/// <summary>
///     A delegate to handle <see cref="PluginDiagnosticsData" /> events.
/// </summary>
public delegate void OnEventCollected(PluginDiagnosticsData diagnosticsData);

/// <summary>
///     Component to collect diagnostics data from plugins. More specifically <see cref="PluginDiagnosticsData" /> events.
/// </summary>
[PublicAPI]
public class PluginDiagnosticsDataCollector : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object?>>, IDisposable {
	/// <summary>
	///     Creates a new instance of <see cref="PluginDiagnosticsDataCollector" />.
	/// </summary>
	/// <param name="onEventCollected">
	///     A delegate to handle <see cref="PluginDiagnosticsData" /> events.
	/// </param>
	/// <param name="sources">
	///     The plugin diagnostic names to collect diagnostics data from.
	/// </param>
	public PluginDiagnosticsDataCollector(OnEventCollected onEventCollected, params string[] sources) {
		OnEventCollected = onEventCollected;
		Sources = [..sources];

		if (sources.Length > 0)
			DiagnosticListener.AllListeners.Subscribe(this);
	}

	/// <summary>
	///     Creates a new instance of <see cref="PluginDiagnosticsDataCollector" />.
	/// </summary>
	/// <param name="sources">
	///     The plugin diagnostic names to collect diagnostics data from.
	/// </param>
	public PluginDiagnosticsDataCollector(params string[] sources) : this(static _ => { }, sources) { }

	ConcurrentDictionary<string, PluginDiagnosticsData> CollectedEventsByPlugin { get; } = new();
	OnEventCollected OnEventCollected { get; }
	List<string> Sources { get; }
	List<IDisposable> Subscriptions { get; } = [];

	/// <summary>
	///		The collected <see cref="PluginDiagnosticsData" /> events.
	/// </summary>
	public ICollection<PluginDiagnosticsData> CollectedEvents => CollectedEventsByPlugin.Values.ToArray();

	void IObserver<DiagnosticListener>.OnNext(DiagnosticListener value) {
		// if (Sources.Contains(value.Name) && value.IsEnabled(value.Name)) 
		if (Sources.Contains(value.Name))
			Subscriptions.Add(value.Subscribe(this));
	}

	void IObserver<KeyValuePair<string, object?>>.OnNext(KeyValuePair<string, object?> value) {
		if (value.Key != nameof(PluginDiagnosticsData) || value.Value is not PluginDiagnosticsData pluginEvent) return;

		CollectedEventsByPlugin.AddOrUpdate(
			pluginEvent.Source,
			static (_, pluginEvent) => pluginEvent,
			static (_, _, pluginEvent) => pluginEvent,
			pluginEvent
		);

		try {
			OnEventCollected(pluginEvent);
		}
		catch (Exception) {
			// stay on target
		}
	}

	void IObserver<DiagnosticListener>.OnCompleted() { }

	void IObserver<KeyValuePair<string, object?>>.OnCompleted() { }

	void IObserver<DiagnosticListener>.OnError(Exception error) { }

	void IObserver<KeyValuePair<string, object?>>.OnError(Exception error) { }

	/// <inheritdoc />
	public void Dispose() {
		foreach (var subscription in Subscriptions)
			subscription.Dispose();
	}

	/// <summary>
	///     Starts the <see cref="PluginDiagnosticsDataCollector" /> with the specified delegate and sources.
	///     This method is a convenient way to create a new instance of the <see cref="PluginDiagnosticsDataCollector" /> and start collecting data immediately.
	/// </summary>
	/// <param name="onEventCollected">
	///     A delegate to handle <see cref="PluginDiagnosticsData" /> events.
	/// </param>
	/// <param name="sources">
	///     The plugin diagnostic names to collect diagnostics data from.
	/// </param>
	public static PluginDiagnosticsDataCollector Start(OnEventCollected onEventCollected, params string[] sources) =>
		new(onEventCollected, sources);

	/// <summary>
	///     Starts the <see cref="PluginDiagnosticsDataCollector" /> with the specified sources.
	///     This method is a convenient way to create a new instance of the <see cref="PluginDiagnosticsDataCollector" /> and start collecting data immediately.
	/// </summary>
	/// <param name="sources">
	///     The plugin diagnostic names to collect diagnostics data from.
	/// </param>
	public static PluginDiagnosticsDataCollector Start(params string[] sources) =>
		new(sources);
}