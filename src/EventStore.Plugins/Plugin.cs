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
        
        IsEnabledResult = (false, "");
        Configuration = null!;
    }
    
    protected Plugin(PluginOptions options) : this(
        options.Name,
        options.Version,
        options.LicensePublicKey,
        options.DiagnosticsName,
        options.DiagnosticsTags) { }
    
    string? LicensePublicKey { get; }

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
    
    /// <inheritdoc />
    public bool Enabled => IsEnabledResult.Enabled;

    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration) { }

    public virtual void ConfigureApplication(IApplicationBuilder app, IConfiguration configuration) { }

    public virtual (bool Enabled, string EnableInstructions) IsEnabled(IConfiguration configuration) => (true, "");

    IServiceCollection IPlugableComponent.ConfigureServices(IServiceCollection services, IConfiguration configuration) {
        Configuration   = configuration;
        IsEnabledResult = IsEnabled(configuration);

        if (Enabled) 
            ConfigureServices(services, configuration);

        return services;
    }
    
    IApplicationBuilder IPlugableComponent.Configure(IApplicationBuilder app) {
        PublishDiagnostics(new() { ["enabled"] = Enabled });
        
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
        
        var license = app.ApplicationServices.GetService<License>();
        if (Enabled && LicensePublicKey is not null && (license is null || !license.IsValid(LicensePublicKey)))
            throw new PluginLicenseException(Name);
        
        if (!Enabled) {
            logger.LogInformation(
                "{Version} plugin disabled. {EnableInstructions}", 
                Version, IsEnabledResult.EnableInstructions
            );

            return app;
        }
      
        logger.LogInformation("{Version} plugin enabled.", Version);

        ConfigureApplication(app, Configuration);
        
        PublishDiagnostics(new() { ["enabled"] = Enabled });

        return app;
    }
    
    protected internal void PublishDiagnostics(string eventName, Dictionary<string, object?> eventData) {
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
    
    protected internal void PublishDiagnostics(Dictionary<string, object?> eventData) =>
        PublishDiagnostics(nameof(PluginDiagnosticsData), eventData);
    
    protected internal void PublishDiagnosticsEvent<T>(T pluginEvent) => 
        DiagnosticListener.Write(typeof(T).Name, pluginEvent);

    /// <inheritdoc />
    public void Dispose() {
        DiagnosticListener.Dispose();
    }
}
