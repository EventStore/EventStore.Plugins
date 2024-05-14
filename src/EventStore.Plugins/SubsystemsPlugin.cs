using System.Diagnostics.Metrics;
using System.Text.Json.Nodes;
using EventStore.Plugins.Subsystems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

// [PublicAPI]
// public abstract class SubsystemsPlugin<T> : ISubsystem, ISubsystemsPlugin where T : SubsystemsPlugin<T>, new() {
//     protected SubsystemsPlugin(
//         string? name = null, string version = "0.0.1", 
//         string? commandLineName = null, string? diagnosticsName = null,
//         params KeyValuePair<string, object?>[] diagnosticsTags) {
//         Name = name ?? typeof(T).Name
//             .Replace("Subsystem", "")
//             .Replace("Plugin", "")
//             .Replace("Component", "");
//
//         Version = version;
//         CommandLineName = commandLineName ?? Name.Underscore().ToLowerInvariant();
//         
//         DiagnosticsName = diagnosticsName ?? Name;
//         DiagnosticsTags = diagnosticsTags;
//         
//         Meter = new(DiagnosticsName, Version, DiagnosticsTags);
//         
//         Enabled = true;
//     }
//
//     protected SubsystemsPlugin(
//         string? name = null, string version = "0.0.1", string? commandLineName = null, string? diagnosticsName = null,
//         params (string Key, object? Value)[] diagnosticsTags) : this(name, version, commandLineName, diagnosticsName, diagnosticsTags.Length > 0
//         ? diagnosticsTags.Select(t => new KeyValuePair<string, object?>(t.Key, t.Value)).ToArray()
//         : []) { }
//     
//     Meter Meter { get; }
//
//     public string Name            { get; }
//     public string Version         { get; }
//     public string CommandLineName { get; }
//     public string DiagnosticsName { get; }
//     public bool   Enabled         { get; protected set; }
//
//     public virtual KeyValuePair<string, object?>[] DiagnosticsTags { get; }
//     
//     public virtual Task Start() => Task.CompletedTask;
//     
//     public virtual Task Stop() => Task.CompletedTask;
//
//     public virtual IReadOnlyList<ISubsystem> GetSubsystems() => [this];
//
//     public virtual IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;
//
//     public virtual IApplicationBuilder Configure(WebHostBuilderContext context, ApplicationBuilder app) => app;
//     
//     public void CollectTelemetry(Action<string, JsonNode> reply) {
//         throw new NotImplementedException();
//     }
// }


[PublicAPI]
public abstract class Plugin {
    protected Plugin(
        string? name = null, string version = "0.0.1", 
        string? commandLineName = null, string? diagnosticsName = null,
        params KeyValuePair<string, object?>[] diagnosticsTags) {
        Name = name ?? GetType().Name
            .Replace("Subsystem", "")
            .Replace("Plugin", "")
            .Replace("Component", "");

        Version = version;
        CommandLineName = commandLineName ?? Name.Underscore().ToLowerInvariant();
        
        DiagnosticsName = diagnosticsName ?? Name;
        DiagnosticsTags = diagnosticsTags;
        
        Meter = new(DiagnosticsName, Version, DiagnosticsTags);
        
        Enabled = true;
    }

    protected Plugin(
        string? name = null, string version = "0.0.1", string? commandLineName = null, string? diagnosticsName = null,
        params (string Key, object? Value)[] diagnosticsTags) : this(name, version, commandLineName, diagnosticsName, diagnosticsTags.Length > 0
        ? diagnosticsTags.Select(t => new KeyValuePair<string, object?>(t.Key, t.Value)).ToArray()
        : []) { }
    
    Meter Meter { get; }

    public string Name            { get; }
    public string Version         { get; }
    public string CommandLineName { get; }
    public string DiagnosticsName { get; }
    public bool   Enabled         { get; protected set; }

    public virtual KeyValuePair<string, object?>[] DiagnosticsTags { get; }
    
    public virtual IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;

    public virtual IApplicationBuilder Configure(WebHostBuilderContext context, ApplicationBuilder app) => app;
    
    public virtual void CollectTelemetry(Action<string, JsonNode> reply) { }
}

[PublicAPI]
public abstract class SubsystemsPlugin<T> : Plugin, ISubsystem, ISubsystemsPlugin where T : SubsystemsPlugin<T>, new() {
    protected SubsystemsPlugin(
        string? name = null, string version = "0.0.1", 
        string? commandLineName = null, string? diagnosticsName = null,
        params KeyValuePair<string, object?>[] diagnosticsTags) : base(name, version, commandLineName, diagnosticsName, diagnosticsTags) { }

    protected SubsystemsPlugin(
        string? name = null, string version = "0.0.1", string? commandLineName = null, string? diagnosticsName = null,
        params (string Key, object? Value)[] diagnosticsTags) : base(name, version, commandLineName, diagnosticsName, diagnosticsTags) { }
    
    public virtual Task Start() => Task.CompletedTask;
    
    public virtual Task Stop() => Task.CompletedTask;

    public virtual IReadOnlyList<ISubsystem> GetSubsystems() => [this];
}
