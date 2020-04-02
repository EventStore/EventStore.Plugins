namespace EventStore.Plugins.Authorization {
	public interface IAuthorizationPlugin {
		string Name { get; }
		string Version { get; }

		string CommandLineName { get; }

		/// <summary>
		/// Creates an authorization provider factory for the authorization plugin
		/// </summary>
		/// <param name="authorizationConfigPath">The path to the configuration file for the authorization plugin</param>
		IAuthorizationProviderFactory GetAuthorizationProviderFactory(string authorizationConfigPath);
	}
}
