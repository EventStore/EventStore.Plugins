using Microsoft.AspNetCore.Http;

namespace EventStore.Plugins.Authentication;

public interface IHttpAuthenticationProvider {
	/// <summary>
	///     Return a unique name used to externally identify the authentication provider.
	/// </summary>
	string Name { get; }

	bool Authenticate(HttpContext context, out HttpAuthenticationRequest request);
}