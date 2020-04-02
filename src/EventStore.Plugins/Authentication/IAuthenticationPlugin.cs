namespace EventStore.Plugins.Authentication {
	public interface IAuthenticationPlugin {
		string Name { get; }
		string Version { get; }

		string CommandLineName { get; }

		/// <summary>
		///     Creates an authentication provider factory for the authentication plugin
		/// </summary>
		/// <param name="authenticationConfigPath">The path to the configuration file for the authentication plugin</param>
		IAuthenticationProviderFactory GetAuthenticationProviderFactory(string authenticationConfigPath);
	}
}
