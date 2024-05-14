using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

/// <summary>
/// Component that can be plugged into the main server.
/// </summary>
public interface IPlugableComponent {
    /// <summary>
    /// The name of the component.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The name used for diagnostics.
    /// </summary>
    string DiagnosticsName { get; }

    /// <summary>
    /// The tags used for diagnostics.
    /// </summary>
    KeyValuePair<string, object?>[] DiagnosticsTags { get; }
    
    /// <summary>
    /// The version of the component.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Indicates whether the component is enabled.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    /// Configures the services using the provided IServiceCollection and IConfiguration.
    /// </summary>
    /// <param name="services">The IServiceCollection to use for configuration.</param>
    /// <param name="configuration">The IConfiguration to use for configuration.</param>
    /// <returns>The configured IServiceCollection.</returns>
    IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration);

    /// <summary>
    /// Configures the application using the provided WebHostBuilderContext and IApplicationBuilder.
    /// </summary>
    /// <param name="context">The WebHostBuilderContext to use for configuration.</param>
    /// <param name="builder">The IApplicationBuilder to use for configuration.</param>
    /// <returns>The configured IApplicationBuilder.</returns>
    IApplicationBuilder Configure(WebHostBuilderContext context, IApplicationBuilder builder);
}