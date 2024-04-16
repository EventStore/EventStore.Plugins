using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace EventStore.Plugins.Authorization {
	public interface IAuthorizationProvider : IConfigurationServices {
		/// <summary>
		///     Check whether the provided <see cref="ClaimsPrincipal" /> has the rights to perform the <see cref="Operation" />
		/// </summary>
		ValueTask<bool> CheckAccessAsync(ClaimsPrincipal cp, Operation operation, CancellationToken ct);
	}
}
