using System.Diagnostics.Metrics;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

[PublicAPI]
public abstract class Plugin : IPlugableComponent {
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
        
        Enabled = true;
    }

    protected Plugin(
        string? name = null, string version = "0.0.1", string? diagnosticsName = null,
        params (string Key, object? Value)[] diagnosticsTags) : this(name, version, diagnosticsName, diagnosticsTags.Length > 0
        ? diagnosticsTags.Select(t => new KeyValuePair<string, object?>(t.Key, t.Value)).ToArray()
        : []) { }
    
    Meter Meter { get; }

    public string Name            { get; }
    public string Version         { get; }
    public string DiagnosticsName { get; }
    public bool   Enabled         { get; protected set; }

    public virtual KeyValuePair<string, object?>[] DiagnosticsTags { get; }
    
    public virtual IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;

    public virtual IApplicationBuilder Configure(WebHostBuilderContext context, ApplicationBuilder app) => app;
    
    public virtual void CollectTelemetry(Action<string, JsonNode> reply) { }
}