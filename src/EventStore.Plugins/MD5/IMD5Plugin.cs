using System;
using System.Text.Json.Nodes;

namespace EventStore.Plugins.MD5;

public interface IMD5Plugin {
	string Name { get; }
	string Version { get; }

	string CommandLineName { get; }

	void CollectTelemetry(Action<string, JsonNode> reply);

	/// <summary>
	///     Creates an MD5 provider factory for the MD5 plugin
	/// </summary>
	IMD5ProviderFactory GetMD5ProviderFactory();
}
