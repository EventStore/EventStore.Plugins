using System.Diagnostics;
using System.Diagnostics.Metrics;
using EventStore.Plugins.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

[PublicAPI]
public abstract class Plugin : IPlugableComponent, IDisposable {
    protected Plugin(
        string? name = null, string version = "0.0.1", 
        string? diagnosticsName = null,
        params KeyValuePair<string, object?>[] diagnosticsTags) {
        Name = name ?? GetType().Name
            .Replace("Subsystem", "")
            .Replace("Plugin", "")
            .Replace("Component", "");

        Version = version;
        
        DiagnosticsName = diagnosticsName ?? Name;
        DiagnosticsTags = diagnosticsTags;
        
        Meter = new(DiagnosticsName, Version, DiagnosticsTags);
        DiagnosticListener = new(DiagnosticsName);
        
        Enabled = true;
    }
    
    protected Meter Meter { get; }
    
    DiagnosticListener DiagnosticListener { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Version { get; }

    /// <inheritdoc />
    public string DiagnosticsName { get; }

    /// <inheritdoc />
    public bool Enabled { get; protected set; }

    /// <inheritdoc />
    public KeyValuePair<string, object?>[] DiagnosticsTags { get; }
    
    /// <inheritdoc />
    public virtual IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;
    
    /// <inheritdoc />
    public virtual IApplicationBuilder Configure(WebHostBuilderContext context, IApplicationBuilder app) => app;
    
    protected internal void PublishDiagnostics(Dictionary<string, object?> eventData) {
        // if (DiagnosticListener.IsEnabled(nameof(PluginDiagnosticsData))) 
        DiagnosticListener.Write(
            nameof(PluginDiagnosticsData), 
            new PluginDiagnosticsData(
                DiagnosticsName, 
                nameof(PluginDiagnosticsData), 
                eventData, 
                DateTimeOffset.UtcNow
            )
        );
    }
    
    protected internal void PublishDiagnosticsEvent<T>(T pluginEvent) => 
        DiagnosticListener.Write(typeof(T).Name, pluginEvent);

    protected internal void PublishDiagnostics(string eventName, Dictionary<string, object?> eventData) {
        // if (DiagnosticListener.IsEnabled(nameof(PluginDiagnosticsData))) 
        DiagnosticListener.Write(
            nameof(PluginDiagnosticsData), 
            new PluginDiagnosticsData(
                DiagnosticsName, 
                eventName, 
                eventData, 
                DateTimeOffset.UtcNow
            )
        );
    }

    /// <inheritdoc />
    public void Dispose() {
        Meter.Dispose();
        DiagnosticListener.Dispose();
    }
}

