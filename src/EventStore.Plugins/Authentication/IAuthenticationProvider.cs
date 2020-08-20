﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace EventStore.Plugins.Authentication {
	public interface IAuthenticationProvider {
		/// <summary>
		///     Initialize the AuthenticationProvider. Event Store will wait until this task completes before becoming ready.
		/// </summary>
		Task Initialize();

		/// <summary>
		///     Authenticate an AuthenticationRequest. Call the appropriate method on <see cref="AuthenticationRequest" />
		///     depending on whether the request succeeded, failed, or errored.
		/// </summary>
		/// <param name="authenticationRequest"></param>
		void Authenticate(AuthenticationRequest authenticationRequest);

		/// <summary>
		///     Return a unique name used to externally identify the authentication provider.
		/// </summary>
		string Name { get; }

		/// <summary>
		///     Get public properties which may be required for the authentication flow.
		/// </summary>
		IEnumerable<KeyValuePair<string,string>> GetPublicProperties();

		/// <summary>
		///     Create any required endpoints.
		/// </summary>
		/// <param name="endpointRouteBuilder"></param>
		void ConfigureEndpoints(IEndpointRouteBuilder endpointRouteBuilder);
	}
}
