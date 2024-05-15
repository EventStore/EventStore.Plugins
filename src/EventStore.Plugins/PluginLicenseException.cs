namespace EventStore.Plugins;

public class PluginLicenseException(string pluginName) : Exception(
    $"A license is required to use the {pluginName} plugin, but was not found. " +
    "Please obtain a license or disable the plugin."
) {
    public string PluginName { get; } = pluginName;
}