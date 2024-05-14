using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

// Component that can be plugged in to the main server.
// Plugins are libraries that result in IPlugableComponents being produced and plugged in.
public interface IPlugableComponent {
    string Name { get; }
    string DiagnosticsName { get; }
    string Version { get; }
    bool Enabled { get; }

    IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;

    IApplicationBuilder Configure(WebHostBuilderContext context, IApplicationBuilder builder) => builder;

    void CollectTelemetry(Action<string, JsonNode> reply) { }
}