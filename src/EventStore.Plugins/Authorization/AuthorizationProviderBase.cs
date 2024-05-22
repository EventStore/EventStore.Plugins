using System.Security.Claims;

namespace EventStore.Plugins.Authorization;

public abstract class AuthorizationProviderBase(PluginOptions options) : Plugin(options), IAuthorizationProvider {
	protected AuthorizationProviderBase(
		string? name = null, 
		string? version = null,
		string? licensePublicKey = null,
		string? diagnosticsName = null,
		params KeyValuePair<string, object?>[] diagnosticsTags
	) : this(new() {
		Name = name,
		Version = version,
		LicensePublicKey = licensePublicKey,
		DiagnosticsName = diagnosticsName,
		DiagnosticsTags = diagnosticsTags
	}) { }
	
	public abstract ValueTask<bool> CheckAccessAsync(ClaimsPrincipal principal, Operation operation, CancellationToken cancellationToken);
}