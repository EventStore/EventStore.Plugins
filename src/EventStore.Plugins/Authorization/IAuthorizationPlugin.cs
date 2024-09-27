// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Authorization;

public interface IAuthorizationPlugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }

	/// <summary>
	///     Creates an authorization provider factory for the authorization plugin
	/// </summary>
	/// <param name="authorizationConfigPath">The path to the configuration file for the authorization plugin</param>
	IAuthorizationProviderFactory GetAuthorizationProviderFactory(string authorizationConfigPath);
}
