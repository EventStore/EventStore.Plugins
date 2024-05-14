namespace EventStore.Plugins.MD5;

public interface IMD5ProviderFactory {
    /// <summary>
    ///     Builds an MD5 provider for the MD5 plugin
    /// </summary>
    IMD5Provider Build();
}