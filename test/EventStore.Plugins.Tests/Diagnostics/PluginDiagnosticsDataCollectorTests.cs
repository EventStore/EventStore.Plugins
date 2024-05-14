using EventStore.Plugins.Diagnostics;
using FluentAssertions;
using Xunit;

namespace EventStore.Plugins.Tests.Diagnostics;

public class PluginDiagnosticsDataCollectorTests {
    [Fact]
    public void can_collect_diagnostics_data_from_plugin() {
        var pluginName = Guid.NewGuid().ToString();
        using var plugin = new TestPlugin(pluginName);

        using var sut = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

        plugin.PublishDiagnostics(new() { ["enabled"] = plugin.Enabled });

        sut.CollectedEvents.Should().ContainSingle().Which
            .Data["enabled"].Should().Be(true);
    }

    [Fact]
    public void can_collect_diagnostics_data_from_subsystems_plugin() {
        var pluginName = Guid.NewGuid().ToString();
        using var plugin = new TestSubsystemsPlugin(pluginName);

        using var sut = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

        plugin.PublishDiagnostics(new() { ["enabled"] = plugin.Enabled });

        sut.CollectedEvents.Should().ContainSingle().Which
            .Data["enabled"].Should().Be(true);
    }

    class TestPlugin(string? pluginName = null) : Plugin(pluginName);

    class TestSubsystemsPlugin(string? pluginName = null) : SubsystemsPlugin(pluginName);
}

