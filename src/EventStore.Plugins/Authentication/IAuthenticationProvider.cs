using System.Threading.Tasks;

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
	}
}
