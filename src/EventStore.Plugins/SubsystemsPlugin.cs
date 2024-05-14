using EventStore.Plugins.Subsystems;

namespace EventStore.Plugins;

[PublicAPI]
public abstract class SubsystemsPlugin<T> : Plugin, ISubsystem, ISubsystemsPlugin where T : SubsystemsPlugin<T>, new() {
    protected SubsystemsPlugin(
        string? name = null, string version = "0.0.1",
        string? commandLineName = null, string? diagnosticsName = null,
        params KeyValuePair<string, object?>[] diagnosticsTags) : base(name, version, diagnosticsName, diagnosticsTags) {
        CommandLineName = commandLineName ?? Name.Underscore().ToLowerInvariant();
    }

    protected SubsystemsPlugin(
        string? name = null, string version = "0.0.1", string? commandLineName = null, string? diagnosticsName = null,
        params (string Key, object? Value)[] diagnosticsTags) : base(name, version, diagnosticsName, diagnosticsTags) {
        CommandLineName = commandLineName ?? Name.Underscore().ToLowerInvariant();
    }
    
    public string CommandLineName { get; }
    
    public virtual Task Start() => Task.CompletedTask;
    
    public virtual Task Stop() => Task.CompletedTask;

    public virtual IReadOnlyList<ISubsystem> GetSubsystems() => [this];
}
