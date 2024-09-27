// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using Microsoft.AspNetCore.Http;

namespace EventStore.Plugins.Authentication;

public interface IHttpAuthenticationProvider {
	/// <summary>
	///     Return a unique name used to externally identify the authentication provider.
	/// </summary>
	string Name { get; }

	bool Authenticate(HttpContext context, out HttpAuthenticationRequest request);
}
