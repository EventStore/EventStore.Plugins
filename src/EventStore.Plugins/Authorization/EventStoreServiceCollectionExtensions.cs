using System;
using EventStore.Plugins.Authorization;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection {
	// ReSharper restore CheckNamespace

	public static class EventStoreServiceCollectionExtensions {
		/// <summary>
		/// Adds the specified <see cref="IAuthorizationProvider"/> to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException"></exception>
		public static IServiceCollection AddAuthorizationProvider(this IServiceCollection services,
			IAuthorizationProvider authorizationProvider) {
			if (services == null) throw new ArgumentNullException(nameof(services));
			if (authorizationProvider == null) throw new ArgumentNullException(nameof(authorizationProvider));

			authorizationProvider.ConfigureServices(services);
			return services.AddSingleton(authorizationProvider);
		}
	}
}
