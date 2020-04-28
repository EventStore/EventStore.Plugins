using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace EventStore.Plugins.Authentication {
	public abstract class AuthenticationRequest {
		private readonly IReadOnlyDictionary<string, string> _tokens;

		/// <summary>
		///     The Identifier for the source that this request came from
		/// </summary>
		public readonly string Id;

		/// <summary>
		///     The name of the principal for the request
		/// </summary>
		public readonly string Name;

		/// <summary>
		///     The supplied password for the request
		/// </summary>
		public readonly string SuppliedPassword;

		protected AuthenticationRequest(string id, IReadOnlyDictionary<string, string> tokens) {
			if (id == null) throw new ArgumentNullException(nameof(id));
			if (tokens == null) throw new ArgumentNullException(nameof(tokens));
			Id = id;
			_tokens = tokens;
			Name = GetToken("uid");
			SuppliedPassword = GetToken("pwd");
		}

		protected AuthenticationRequest(string id, string name, string suppliedPassword)
			: this(id, new Dictionary<string, string> {
				["uid"] = name,
				["pwd"] = suppliedPassword
			}) {
		}

		/// <summary>
		/// Gets the token corresponding to <param name="key" />.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetToken(string key) => _tokens.TryGetValue(key, out var token) ? token : null;

		/// <summary>
		///     The request is unauthorized
		/// </summary>
		public abstract void Unauthorized();

		/// <summary>
		///     The request was successfully authenticated
		/// </summary>
		/// <param name="principal">The <see cref="ClaimsPrincipal" /> of the authenticated request</param>
		public abstract void Authenticated(ClaimsPrincipal principal);

		/// <summary>
		///     An error occurred during authentication
		/// </summary>
		public abstract void Error();

		/// <summary>
		///     The authentication provider is not yet ready to service the request
		/// </summary>
		public abstract void NotReady();
	}
}
