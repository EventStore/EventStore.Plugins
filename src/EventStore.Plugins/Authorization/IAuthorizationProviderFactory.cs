namespace EventStore.Plugins.Authorization;

public interface IAuthorizationProviderFactory {
	/// <summary>
	///     Build an AuthorizationProvider for the authorization plugin
	/// </summary>
	IAuthorizationProvider Build();
}
