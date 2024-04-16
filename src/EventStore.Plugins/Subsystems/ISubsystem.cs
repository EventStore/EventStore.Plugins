using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins.Subsystems;

public interface ISubsystem : IConfigureServices {
	string Name { get; }
	void CollectTelemetry(Action<string, JsonNode> reply);
	Task Start();
	Task Stop();
}
