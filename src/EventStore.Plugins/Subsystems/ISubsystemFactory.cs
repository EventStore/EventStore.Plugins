namespace EventStore.Plugins.Subsystems;

public interface ISubsystemFactory<TArg> {
	ISubsystem Create(TArg arg);
}
