// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Security.Claims;

namespace EventStore.Plugins.Authorization;

public abstract class AuthorizationProviderBase(PluginOptions options) : Plugin(options), IAuthorizationProvider {
	protected AuthorizationProviderBase(
		string? name = null,
		string? version = null,
		string? licensePublicKey = null,
		string[]? requiredEntitlements = null,
		string? diagnosticsName = null,
		params KeyValuePair<string, object?>[] diagnosticsTags
	) : this(new() {
		Name = name,
		Version = version,
		LicensePublicKey = licensePublicKey,
		RequiredEntitlements = requiredEntitlements,
		DiagnosticsName = diagnosticsName,
		DiagnosticsTags = diagnosticsTags
	}) { }
	
	public abstract ValueTask<bool> CheckAccessAsync(ClaimsPrincipal principal, Operation operation, CancellationToken cancellationToken);
}
