using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace EventStore.Plugins.Authorization {
	public interface IAuthorizationProvider {
		/// <summary>
		///     Check whether the provided <see cref="ClaimsPrincipal" /> has the rights to perform the <see cref="Operation" />
		/// </summary>
		ValueTask<bool> CheckAccessAsync(ClaimsPrincipal cp, Operation operation, CancellationToken ct);

		/// <summary>
		/// Configures the <see cref="IEndpointRouteBuilder"/>.
		/// </summary>
		void ConfigureEndpoints(IEndpointRouteBuilder builder) { }

		/// <summary>
		/// Configures the <see cref="IServiceCollection"/>
		/// </summary>
		void ConfigureServices(IServiceCollection services) { }
	}
}
