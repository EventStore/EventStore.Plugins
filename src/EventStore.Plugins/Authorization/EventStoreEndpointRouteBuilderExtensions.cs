using System;
using EventStore.Plugins.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder {
	public static class EventStoreEndpointRouteBuilderExtensions {
		/// <summary>
		/// Configures the endpoints specified in the <see cref="IAuthorizationProvider"/>.
		/// </summary>
		/// <returns><see cref="IApplicationBuilder"/></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static IApplicationBuilder UseAuthorizationProvider(this IApplicationBuilder builder) {
			if (builder == null) throw new ArgumentNullException(nameof(builder));
			var provider = builder.ApplicationServices.GetRequiredService<IAuthorizationProvider>();

			return builder.UseEndpoints(provider.ConfigureEndpoints).UseAuthorization();
		}
	}
}
