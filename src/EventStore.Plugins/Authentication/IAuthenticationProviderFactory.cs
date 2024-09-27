// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Authentication;

public interface IAuthenticationProviderFactory {
	/// <summary>
	///     Build an AuthenticationProvider for the authentication plugin
	/// </summary>
	/// <param name="logFailedAuthenticationAttempts">
	///     Whether the Authentication Provider should log failed authentication attempts
	/// </param>
	IAuthenticationProvider Build(bool logFailedAuthenticationAttempts);
}
