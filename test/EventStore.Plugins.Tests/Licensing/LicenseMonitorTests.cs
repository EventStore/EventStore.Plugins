using EventStore.Plugins.Licensing;
using Microsoft.Extensions.Logging.Testing;

namespace EventStore.Plugins.Tests.Licensing;

public class LicenseMonitorTests {
	[Fact]
	public async Task valid_license_with_correct_entitlements() {
		var licenseService = new PluginBaseTests.FakeLicenseService(
			createLicense: true,
			entitlements: ["MY_ENTITLEMENT"]);

		var criticalError = false;

		using var subscription = await LicenseMonitor.MonitorAsync(
			featureName: "TestFeature",
			requiredEntitlements: ["MY_ENTITLEMENT"],
			licenseService: licenseService,
			logger: new FakeLogger(),
			licensePublicKey: licenseService.PublicKey,
			onCriticalError: _ => criticalError = true);

		licenseService.RejectionException.Should().BeNull();
		criticalError.Should().BeFalse();
	}

	[Fact]
	public async Task valid_license_with_all_entitlement() {
		var licenseService = new PluginBaseTests.FakeLicenseService(
			createLicense: true,
			entitlements: ["ALL"]);

		var criticalError = false;

		using var subscription = await LicenseMonitor.MonitorAsync(
			featureName: "TestFeature",
			requiredEntitlements: ["MY_ENTITLEMENT"],
			licenseService: licenseService,
			logger: new FakeLogger(),
			licensePublicKey: licenseService.PublicKey,
			onCriticalError: _ => criticalError = true);

		licenseService.RejectionException.Should().BeNull();
		criticalError.Should().BeFalse();
	}

	[Fact]
	public async Task valid_license_with_missing_entitlement() {
		var licenseService = new PluginBaseTests.FakeLicenseService(
			createLicense: true,
			entitlements: []);

		var criticalError = false;

		using var subscription = await LicenseMonitor.MonitorAsync(
			featureName: "TestFeature",
			requiredEntitlements: ["MY_ENTITLEMENT"],
			licenseService: licenseService,
			logger: new FakeLogger(),
			licensePublicKey: licenseService.PublicKey,
			onCriticalError: _ => criticalError = true);

		licenseService.RejectionException.Should().BeOfType<LicenseEntitlementException>()
			.Which.MissingEntitlement.Should().Be("MY_ENTITLEMENT");
		criticalError.Should().BeFalse();
	}

	[Fact]
	public async Task no_license() {
		var licenseService = new PluginBaseTests.FakeLicenseService(
			createLicense: false);

		var criticalError = false;

		using var subscription = await LicenseMonitor.MonitorAsync(
			featureName: "TestFeature",
			requiredEntitlements: [],
			licenseService: licenseService,
			logger: new FakeLogger(),
			licensePublicKey: licenseService.PublicKey,
			onCriticalError: _ => criticalError = true);

		licenseService.RejectionException.Should().BeOfType<LicenseException>();
		criticalError.Should().BeFalse();
	}

	[Fact]
	public async Task license_is_not_valid() {
		var licenseService = new PluginBaseTests.FakeLicenseService(
			createLicense: true,
			entitlements: []);

		var criticalError = false;

		using var subscription = await LicenseMonitor.MonitorAsync(
			featureName: "TestFeature",
			requiredEntitlements: [],
			licenseService: licenseService,
			logger: new FakeLogger(),
			licensePublicKey: "a_different_public_key",
			onCriticalError: _ => criticalError = true);

		licenseService.RejectionException.Should().BeOfType<LicenseException>();
		criticalError.Should().BeTrue();
	}
}
