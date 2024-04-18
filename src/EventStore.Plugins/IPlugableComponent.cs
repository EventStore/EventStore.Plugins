using System.Text.Json.Nodes;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins;

// Component that can be plugged in to the main server.
// Plugins are libraries that result in IPlugableComponents being produced and plugged in.
public interface IPlugableComponent {
	IApplicationBuilder Configure(IApplicationBuilder builder) => builder;
	IServiceCollection ConfigureServices(IServiceCollection services, IConfiguration configuration) => services;
	void CollectTelemetry(Action<string, JsonNode> reply) { }
}
