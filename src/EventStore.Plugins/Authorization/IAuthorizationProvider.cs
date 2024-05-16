using System.Security.Claims;

namespace EventStore.Plugins.Authorization;

public interface IAuthorizationProvider : IPlugableComponent {
	/// <summary>
	///     Check whether the provided <see cref="ClaimsPrincipal" /> has the rights to perform the <see cref="Operation" />
	/// </summary>
	ValueTask<bool> CheckAccessAsync(ClaimsPrincipal cp, Operation operation, CancellationToken ct);
}