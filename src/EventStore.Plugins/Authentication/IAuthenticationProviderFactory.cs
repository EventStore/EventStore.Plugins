namespace EventStore.Plugins.Authentication;

public interface IAuthenticationProviderFactory {
	/// <summary>
	///     Build an AuthenticationProvider for the authentication plugin
	/// </summary>
	/// <param name="logFailedAuthenticationAttempts">
	///     Whether the Authentication Provider should log failed authentication attempts
	/// </param>
	IAuthenticationProvider Build(bool logFailedAuthenticationAttempts);
}