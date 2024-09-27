// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Security.Claims;

namespace EventStore.Plugins.Authorization;

public interface IAuthorizationProvider : IPlugableComponent {
	/// <summary>
	///     Check whether the provided <see cref="ClaimsPrincipal" /> has the rights to perform the <see cref="Operation" />
	/// </summary>
	ValueTask<bool> CheckAccessAsync(ClaimsPrincipal cp, Operation operation, CancellationToken ct);
}
