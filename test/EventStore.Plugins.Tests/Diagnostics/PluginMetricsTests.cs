using System.Diagnostics.Metrics;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Xunit;

namespace EventStore.Plugins.Tests.Diagnostics;

public class PluginMetricsTests {
    [Fact]
    public void can_receive_metrics_from_plugin() {
        // Arrange
        using var plugin = new TestPlugin(diagnosticsTags: new KeyValuePair<string, object?>("test_name", "can_receive_metrics_from_plugin"));

        using var collector = new MetricCollector<int>(null, plugin.DiagnosticsName, plugin.TestCounter.Name);

        // Act
        
        // we also need to add then here? ffs... they should propagate from the meter...
        plugin.TestCounter.Add(1, plugin.DiagnosticsTags); 
        
        // Assert
        var collectedMeasurement = collector.GetMeasurementSnapshot().Should().ContainSingle().Which;
        
        collectedMeasurement.Value.Should().Be(1);
        
        collectedMeasurement.Tags.Should().BeEquivalentTo(plugin.DiagnosticsTags);
    }
    
    class TestPlugin : Plugin{
        public TestPlugin(string? pluginName = null, params KeyValuePair<string, object?>[] diagnosticsTags) : base(pluginName, diagnosticsTags: diagnosticsTags) {
            TestCounter = Meter.CreateCounter<int>(
                name: "plugin_test_calls", 
                unit: "int", 
                description: "just to test the counter", 
                tags: DiagnosticsTags
            );
        }
    
        public Counter<int> TestCounter { get; }
    }
}