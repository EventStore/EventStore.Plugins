// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.MD5;

public interface IMD5Plugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }

	/// <summary>
	///     Creates an MD5 provider factory for the MD5 plugin
	/// </summary>
	IMD5ProviderFactory GetMD5ProviderFactory();
}
