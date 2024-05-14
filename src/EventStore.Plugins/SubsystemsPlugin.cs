using System.Text.RegularExpressions;
using EventStore.Plugins.Subsystems;

namespace EventStore.Plugins;

[PublicAPI]
public abstract class SubsystemsPlugin : Plugin, ISubsystem, ISubsystemsPlugin {
    protected SubsystemsPlugin(
        string? name = null, string version = "0.0.1",
        string? commandLineName = null, string? diagnosticsName = null,
        params KeyValuePair<string, object?>[] diagnosticsTags) : base(name, version, diagnosticsName, diagnosticsTags) {
        CommandLineName = commandLineName ?? Name.Underscore().ToLowerInvariant();
    }
    
    public string CommandLineName { get; }
    
    public virtual Task Start() => Task.CompletedTask;
    
    public virtual Task Stop() => Task.CompletedTask;

    public virtual IReadOnlyList<ISubsystem> GetSubsystems() => [this];
}


static class InflectorExtensions {
    public static string Underscore(this string input) {
        return Regex.Replace(
            Regex.Replace(
                Regex.Replace(input, @"([\p{Lu}]+)([\p{Lu}][\p{Ll}])", "$1_$2"), @"([\p{Ll}\d])([\p{Lu}])",
                "$1_$2"
            ),
            @"[-\s]", "_"
        ).ToLower();
    }
}