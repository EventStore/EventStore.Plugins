namespace EventStore.Plugins;

public class LicenseException(string featureName, Exception? inner = null) : Exception(
	$"A license is required to use the {featureName} feature, but was not found. " +
	"Please obtain a license or disable the feature.",
	inner
) {
	public string FeatureName { get; } = featureName;
}

public class LicenseEntitlementException(string featureName, string entitlement) : Exception(
	$"{featureName} feature requires the {entitlement} entitlement. Please contact EventStore support.") {
	public string FeatureName { get; } = featureName;
	public string MissingEntitlement { get; } = entitlement;
}