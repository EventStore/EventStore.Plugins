﻿namespace EventStore.Plugins.Subsystems;

// A plugin that can create multiple subsystems.
public interface ISubsystemsPlugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }
	IReadOnlyList<ISubsystem> GetSubsystems();
}
