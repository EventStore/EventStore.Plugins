using Microsoft.Extensions.Logging;

namespace EventStore.Plugins.Authentication;

public interface IAuthenticationProviderFactory {
    /// <summary>
    ///     Build an AuthenticationProvider for the authentication plugin
    /// </summary>
    /// <param name="logFailedAuthenticationAttempts">
    ///     Whether the Authentication Provider should log failed authentication
    ///     attempts
    /// </param>
    /// <param name="logger">The <see cref="ILogger" /> to use when logging in the plugin</param>
    IAuthenticationProvider Build(bool logFailedAuthenticationAttempts, ILogger logger);
}