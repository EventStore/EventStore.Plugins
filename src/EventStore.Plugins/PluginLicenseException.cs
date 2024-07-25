namespace EventStore.Plugins;

public class PluginLicenseException(string pluginName, Exception? inner = null) : Exception(
	$"A license is required to use the {pluginName} plugin, but was not found. " +
	"Please obtain a license or disable the plugin.",
	inner
) {
	public string PluginName { get; } = pluginName;
}

public class PluginLicenseEntitlementException(string pluginName, string entitlement) : Exception(
	$"{pluginName} plugin requires the {entitlement} entitlement. Please contact EventStore support.") {
	public string PluginName { get; } = pluginName;
}
