using System.Diagnostics;
using EventStore.Plugins.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.StringComparison;
using License = EventStore.Plugins.Licensing.License;

namespace EventStore.Plugins;

public record PluginOptions {
	public string? Name { get; init; }
	public string? Version { get; init; }
	public string? LicensePublicKey { get; init; }
	public string? DiagnosticsName { get; init; }
	public KeyValuePair<string, object?>[] DiagnosticsTags { get; init; } = [];
}

[PublicAPI]
public abstract class Plugin : IPlugableComponent, IDisposable {
	protected Plugin(
		string? name = null,
		string? version = null,
		string? licensePublicKey = null,
		string? diagnosticsName = null,
		params KeyValuePair<string, object?>[] diagnosticsTags) {
		var pluginType = GetType();

		Name = name ?? pluginType.Name
			.Replace("Plugin", "", OrdinalIgnoreCase)
			.Replace("Component", "", OrdinalIgnoreCase)
			.Replace("Subsystems", "", OrdinalIgnoreCase)
			.Replace("Subsystem", "", OrdinalIgnoreCase);

		Version = version
		          ?? pluginType.Assembly.GetName().Version?.ToString()
		          ?? "1.0.0.0-preview";

		LicensePublicKey = licensePublicKey;

		DiagnosticsName = diagnosticsName ?? Name;
		DiagnosticsTags = diagnosticsTags;

		DiagnosticListener = new(DiagnosticsName);

		IsEnabledResult = (true, "");
		Configuration = null!;

		LastDiagnosticsDataSnapshot = new() { ["enabled"] = true };
	}

	protected Plugin(PluginOptions options) : this(
		options.Name,
		options.Version,
		options.LicensePublicKey,
		options.DiagnosticsName,
		options.DiagnosticsTags) { }

	public string? LicensePublicKey { get; }

	DiagnosticListener DiagnosticListener { get; }

	(bool Enabled, string EnableInstructions) IsEnabledResult { get; set; }

	IConfiguration Configuration { get; set; }

	/// <inheritdoc />
	public string Name { get; }

	/// <inheritdoc />
	public string Version { get; }

	/// <inheritdoc />
	public string DiagnosticsName { get; }

	/// <inheritdoc />
	public KeyValuePair<string, object?>[] DiagnosticsTags { get; }

	/// <summary>
	/// The last snapshot of diagnostics data published by the plugin.
	/// </summary>
	public Dictionary<string, object?> LastDiagnosticsDataSnapshot { get; private set; }
	
	/// <inheritdoc />
	public bool Enabled => IsEnabledResult.Enabled;

	public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }

	public virtual void ConfigureApplication(IApplicationBuilder app, IConfiguration configuration) { }

	/// <summary>
	/// This check will happen before the plugin is configured and returns true by default.<br/>
	/// Nonetheless the plugin can still be disabled by calling <see cref="Disable"/> on ConfigureServices and ConfigureApplication.
	/// </summary>
	/// <param name="configuration">The configuration of the application.<br/></param>
	public virtual (bool Enabled, string EnableInstructions) IsEnabled(IConfiguration configuration) => IsEnabledResult;
	
	public void Disable(string reason) => IsEnabledResult = (false, reason);
	
	void IPlugableComponent.ConfigureServices(IServiceCollection services, IConfiguration configuration) {
		Configuration = configuration;
		IsEnabledResult = IsEnabled(configuration);
		
		if (Enabled)
			ConfigureServices(services, configuration);
		
		PublishDiagnosticsData(new(LastDiagnosticsDataSnapshot) {
			["enabled"] = Enabled
		});
	}

	void IPlugableComponent.ConfigureApplication(IApplicationBuilder app, IConfiguration configuration) {
		var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
		
		// if the plugin is disabled, just get out
		if (!Enabled) {
			logger.LogInformation(
				"{Version} plugin disabled. {EnableInstructions}",
				Version, IsEnabledResult.EnableInstructions
			);

			return;
		}
		
		// if the plugin is enabled, but the license is invalid, throw an exception and effectivly disable the plugin
		var license = app.ApplicationServices.GetService<License>();
		if (Enabled && LicensePublicKey is not null && (license is null || !license.IsValid(LicensePublicKey))) {
			var ex = new PluginLicenseException(Name);
			
			IsEnabledResult = (false, ex.Message);
			
			PublishDiagnosticsData(new(LastDiagnosticsDataSnapshot) {
				["enabled"] = Enabled
			});
			
			logger.LogInformation(
				"{Version} plugin disabled. {EnableInstructions}",
				Version, IsEnabledResult.EnableInstructions
			);
			
			throw ex;
		}
		
		// there is still a chance to disable the plugin when configuring the application
		// this is useful when the plugin is enabled, but some conditions that can only be checked here are not met
		ConfigureApplication(app, Configuration);

		// at this point we know if the plugin is enabled and configured or not
		if (Enabled)
			logger.LogInformation("{Version} plugin enabled.", Version);
		else {
			logger.LogInformation(
				"{Version} plugin disabled. {EnableInstructions}",
				Version, IsEnabledResult.EnableInstructions
			);
		}
		
		// finally publish diagnostics data
		PublishDiagnosticsData(new(LastDiagnosticsDataSnapshot) {
			["enabled"] = Enabled
		});
	}
	
	/// <summary>
	///   Publishes diagnostics data as a snapshot.<br/>
	///   Uses the <see cref="PluginDiagnosticsData"/> container.<br/>
	///   Multiple calls to this method will overwrite the previous snapshot.<br/>
	///   Used for ESDB telemetry.
	/// </summary>
	/// <param name="eventData">The data to publish.</param>
	protected internal void PublishDiagnosticsData(Dictionary<string, object?> eventData) {
		var value = new PluginDiagnosticsData {
			Source = DiagnosticsName,
			Data = eventData,
			IsSnapshot = true
		};
		
		DiagnosticListener.Write(nameof(PluginDiagnosticsData), value);
		
		LastDiagnosticsDataSnapshot = value.Data;
	}
	
	/// <summary>
	///   Publishes diagnostics data. <br/>
	///   Uses the same <see cref="PluginDiagnosticsData"/> container as snapshot diagnostics. <br/>
	///   The event 'PluginDiagnosticsData' is reserved for the default snapshot diagnostics data.
	/// </summary>
	/// <param name="eventName">The name of the event to publish.</param>
	/// <param name="eventData">The data to publish.</param>
	/// <param name="isSnapshot">Whether the event is a snapshot and should override previously collected data, by event name.</param>
	protected internal void PublishDiagnosticsData(string eventName, Dictionary<string, object?> eventData, bool isSnapshot = false) {
		if (eventName == nameof(PluginDiagnosticsData)) 
		    throw new ArgumentException("Event name cannot be PluginDiagnosticsData", nameof(eventName));
		
		DiagnosticListener.Write(
			eventName,
			new PluginDiagnosticsData{
				Source = DiagnosticsName,
				EventName = eventName,
				Data = eventData,
				IsSnapshot = isSnapshot
			}
		);
	}
	
	/// <summary>
	///  Publishes diagnostics events. <br/>
	/// </summary>
	/// <param name="pluginEvent"></param>
	/// <typeparam name="T"></typeparam>
	protected internal void PublishDiagnosticsEvent<T>(T pluginEvent) =>
		DiagnosticListener.Write(typeof(T).Name, pluginEvent);

	/// <inheritdoc />
	public void Dispose() => DiagnosticListener.Dispose();
}
