// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.MD5;

public interface IMD5ProviderFactory {
	/// <summary>
	///     Builds an MD5 provider for the MD5 plugin
	/// </summary>
	IMD5Provider Build();
}
