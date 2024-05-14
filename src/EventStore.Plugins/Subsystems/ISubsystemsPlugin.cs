namespace EventStore.Plugins.Subsystems;

/// <summary>
/// A plugin that can create multiple subsystems.
/// </summary>
public interface ISubsystemsPlugin {
	string Name { get; }
	string Version { get; }
	string CommandLineName { get; }
	
    IReadOnlyList<ISubsystem> GetSubsystems();
}
