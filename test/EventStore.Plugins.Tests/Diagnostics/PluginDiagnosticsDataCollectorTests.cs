using EventStore.Plugins.Diagnostics;

namespace EventStore.Plugins.Tests.Diagnostics;

public class PluginDiagnosticsDataCollectorTests {
    [Fact]
    public void can_collect_diagnostics_data_from_plugin() {
        using var plugin = new TestPlugin(pluginName: Guid.NewGuid().ToString());
        
        using var sut = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

        plugin.PublishDiagnostics(new() { ["enabled"] = plugin.Enabled });

        sut.CollectedEvents.Should().ContainSingle().Which
            .Data["enabled"].Should().Be(plugin.Enabled);
    }

    [Fact]
    public void can_collect_diagnostics_data_from_subsystems_plugin() {
        using var plugin = new TestSubsystemsPlugin(pluginName: Guid.NewGuid().ToString());

        using var sut = PluginDiagnosticsDataCollector.Start(plugin.DiagnosticsName);

        plugin.PublishDiagnostics(new() { ["enabled"] = plugin.Enabled });

        sut.CollectedEvents.Should().ContainSingle().Which
            .Data["enabled"].Should().Be(plugin.Enabled);
    }

    class TestPlugin(string? pluginName = null) : Plugin(pluginName);

    class TestSubsystemsPlugin(string? pluginName = null) : SubsystemsPlugin(pluginName);
}

