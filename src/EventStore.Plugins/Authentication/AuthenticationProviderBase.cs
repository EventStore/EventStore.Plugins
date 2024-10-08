// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using Microsoft.AspNetCore.Routing;

namespace EventStore.Plugins.Authentication;

public abstract class AuthenticationProviderBase(PluginOptions options) : Plugin(options), IAuthenticationProvider {
	protected AuthenticationProviderBase(
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

	public virtual Task Initialize() => Task.CompletedTask;

	public abstract void Authenticate(AuthenticationRequest authenticationRequest);

	public virtual IEnumerable<KeyValuePair<string, string>> GetPublicProperties() => [];
	
	public virtual void ConfigureEndpoints(IEndpointRouteBuilder endpointRouteBuilder) { }

	public virtual IReadOnlyList<string> GetSupportedAuthenticationSchemes() => [];
}
