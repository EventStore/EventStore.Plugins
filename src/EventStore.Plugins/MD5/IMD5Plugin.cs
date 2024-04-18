namespace EventStore.Plugins.MD5;

public interface IMD5Plugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }

	/// <summary>
	///     Creates an MD5 provider factory for the MD5 plugin
	/// </summary>
	IMD5ProviderFactory GetMD5ProviderFactory();
}
