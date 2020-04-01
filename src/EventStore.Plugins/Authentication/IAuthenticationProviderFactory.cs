using Serilog;

namespace EventStore.Plugins.Authentication {
	public interface IAuthenticationProviderFactory {
		IAuthenticationProvider Build(bool logFailedAuthenticationAttempts, ILogger logger);
	}
}
