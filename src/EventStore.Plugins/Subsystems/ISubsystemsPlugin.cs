using System.Collections.Generic;

namespace EventStore.Plugins.Subsystems;

// A plugin that can create multiple subsystems.
// A TArg is required to produce each subsystem from its respective factory.
public interface ISubsystemsPlugin<TArg> {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }
	IEnumerable<ISubsystemFactory<TArg>> GetSubsystemFactories(string configPath);
}
