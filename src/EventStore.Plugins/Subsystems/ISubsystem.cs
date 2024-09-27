namespace EventStore.Plugins.Subsystems;

public interface ISubsystem : IPlugableComponent {
	Task Start();
	Task Stop();
}
