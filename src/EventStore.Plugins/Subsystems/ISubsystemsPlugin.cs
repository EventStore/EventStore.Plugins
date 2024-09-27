// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Subsystems;

/// <summary>
///     A plugin that can create multiple subsystems.
/// </summary>
public interface ISubsystemsPlugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }

	IReadOnlyList<ISubsystem> GetSubsystems();
}
