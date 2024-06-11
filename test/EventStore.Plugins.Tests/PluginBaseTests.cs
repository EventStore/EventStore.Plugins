// ReSharper disable AccessToDisposedClosure

using System.Security.Cryptography;
using EventStore.Plugins.Diagnostics;
using EventStore.Plugins.Licensing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Convert;

namespace EventStore.Plugins.Tests;

public class PluginBaseTests {
	[Fact]
	public void plugin_base_sets_defaults_automatically() {
		var expectedOptions = new PluginOptions {
			Name = "NightCity",
			Version = "1.0.0.0",
			DiagnosticsName = "NightCity"
		};

		using var plugin = new NightCityPlugin();

		plugin.Options.Should().BeEquivalentTo(expectedOptions);
	}

	[Fact]
	public void subsystems_plugin_base_sets_defaults_automatically() {
		var expectedOptions = new SubsystemsPluginOptions {
			Name = "PhantomLiberty",
			Version = "1.0.0.0",
			DiagnosticsName = "PhantomLiberty",
			CommandLineName = "phantom-liberty"
		};

		using var plugin = new PhantomLibertySubsystemsPlugin();

		plugin.Options.Should().BeEquivalentTo(expectedOptions);
	}

	[Fact]
	public void plugin_diagnostics_snapshot_is_not_overriden_internally() {
		// Arrange
		var userDiagnosticsData = new Dictionary<string, object?> {
			["first_value"]  = 1,
			["second_value"] = 2
		};

		IPlugableComponent plugin = new NightCityPlugin(new(){ Name = Guid.NewGuid().ToString() }) {
			OnConfigureServices = x => x.PublishDiagnosticsData(userDiagnosticsData),
			OnConfigureApplication = x => x.Disable("Disabled on ConfigureApplication because I can")
		};

		using var collector = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

		var builder = WebApplication.CreateBuilder();

		plugin.ConfigureServices(builder.Services, builder.Configuration);

		using var app = builder.Build();

		// Act & Assert
		plugin.ConfigureApplication(app, app.Configuration);

		var expectedDiagnosticsData = new Dictionary<string, object?>(userDiagnosticsData) {
			["enabled"] = false
		};

		collector.CollectedEvents(plugin.DiagnosticsName).Should().ContainSingle()
			.Which.Data.Should().BeEquivalentTo(expectedDiagnosticsData);
	}

	[Fact]
	public void comercial_plugin_is_disabled_when_licence_is_missing() {
		// Arrange
		IPlugableComponent plugin = new NightCityPlugin(new() {
			LicensePublicKey = "valid-public-key"
		});

		var builder = WebApplication.CreateBuilder();

		plugin.ConfigureServices(builder.Services, builder.Configuration);

		using var app = builder.Build();

		var configure = () => plugin.ConfigureApplication(app, app.Configuration);

		// Act & Assert
		configure.Should().Throw<PluginLicenseException>().Which
			.PluginName.Should().Be(plugin.Name);
	}

	[Fact]
	public void comercial_plugin_is_disabled_when_licence_is_invalid() {
		// Arrange
		var (license, _) = CreateLicense();
		var (_, invalidPublicKey) = CreateLicense();

		IPlugableComponent plugin = new NightCityPlugin(new() {
			LicensePublicKey = invalidPublicKey
		});

		var builder = WebApplication.CreateBuilder();

		builder.Services.AddSingleton(license);

		plugin.ConfigureServices(builder.Services, builder.Configuration);

		using var app = builder.Build();

		var configure = () => plugin.ConfigureApplication(app, app.Configuration);

		// Act & Assert
		configure.Should().Throw<PluginLicenseException>().Which
			.PluginName.Should().Be(plugin.Name);
	}

	[Fact]
	public void comercial_plugin_is_enabled_when_licence_is_present() {
		// Arrange
		var (license, publicKey) = CreateLicense();

		IPlugableComponent plugin = new NightCityPlugin(new() {
			LicensePublicKey = publicKey
		});

		var builder = WebApplication.CreateBuilder();

		builder.Services.AddSingleton(license);

		plugin.ConfigureServices(builder.Services, builder.Configuration);

		using var app = builder.Build();

		var configure = () => plugin.ConfigureApplication(app, builder.Configuration);

		// Act & Assert
		configure.Should().NotThrow<Exception>();
	}

	[Fact]
	public void plugin_can_be_disabled_on_ConfigureServices() {
		// Arrange
		IPlugableComponent plugin = new NightCityPlugin(new(){ Name = Guid.NewGuid().ToString() }) {
			OnConfigureServices = x => x.Disable("Disabled on ConfigureServices because I can")
		};

		using var collector = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

		var builder = WebApplication.CreateBuilder();

		// Act & Assert
		plugin.Enabled.Should().BeTrue();

		plugin.ConfigureServices(builder.Services, builder.Configuration);

		plugin.Enabled.Should().BeFalse();

		collector.CollectedEvents(plugin.DiagnosticsName).Should().ContainSingle()
			.Which.Data.Should().ContainKey("enabled")
			.WhoseValue.Should().BeEquivalentTo(false);
	}

	[Fact]
	public void plugin_can_be_disabled_on_ConfigureApplication() {
		// Arrange
		IPlugableComponent plugin = new NightCityPlugin(new(){ Name = Guid.NewGuid().ToString() }) {
			OnConfigureApplication = x => x.Disable("Disabled on ConfigureApplication because I can")
		};

		using var collector = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

		var builder = WebApplication.CreateBuilder();

		plugin.ConfigureServices(builder.Services, builder.Configuration);

		using var app = builder.Build();

		// Act & Assert
		plugin.Enabled.Should().BeTrue();

		plugin.ConfigureApplication(app, app.Configuration);

		plugin.Enabled.Should().BeFalse();

		collector.CollectedEvents(plugin.DiagnosticsName).Should().ContainSingle()
			.Which.Data.Should().ContainKey("enabled")
			.WhoseValue.Should().BeEquivalentTo(false);
	}

	static (License License, string PublicKey) CreateLicense(Dictionary<string, object>? claims = null) {
		using var rsa = RSA.Create(1024);

		var publicKey = ToBase64String(rsa.ExportRSAPublicKey());
		var privateKey = ToBase64String(rsa.ExportRSAPrivateKey());

		return (License.Create(publicKey, privateKey, claims), publicKey);
	}

	class NightCityPlugin : Plugin {
		public NightCityPlugin(PluginOptions options) : base(options) {
			Options = options with {
				Name = Name,
				Version = Version,
				DiagnosticsName = DiagnosticsName
			};
		}

		public NightCityPlugin() : this(new()) { }

		public PluginOptions Options { get; }

		public Action<Plugin>? OnConfigureServices    { get; set; }
		public Action<Plugin>? OnConfigureApplication { get; set; }

		public override void ConfigureServices(IServiceCollection services, IConfiguration configuration) =>
			OnConfigureServices?.Invoke(this);

		public override void ConfigureApplication(IApplicationBuilder app, IConfiguration configuration) =>
			OnConfigureApplication?.Invoke(this);
	}

	class PhantomLibertySubsystemsPlugin : SubsystemsPlugin {
		public PhantomLibertySubsystemsPlugin(SubsystemsPluginOptions options) : base(options) {
			Options = options with {
				Name = Name,
				Version = Version,
				DiagnosticsName = DiagnosticsName,
				CommandLineName = CommandLineName
			};
		}

		public PhantomLibertySubsystemsPlugin() : this(new()) { }

		public SubsystemsPluginOptions Options { get; }
	}
}