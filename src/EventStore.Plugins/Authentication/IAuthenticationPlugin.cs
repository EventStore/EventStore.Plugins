// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Authentication;

public interface IAuthenticationPlugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }

	/// <summary>
	///     Creates an authentication provider factory for the authentication plugin
	/// </summary>
	/// <param name="authenticationConfigPath">The path to the configuration file for the authentication plugin</param>
	IAuthenticationProviderFactory GetAuthenticationProviderFactory(string authenticationConfigPath);
}
