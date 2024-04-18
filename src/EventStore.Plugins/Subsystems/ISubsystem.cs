using System.Threading.Tasks;

namespace EventStore.Plugins.Subsystems;

public interface ISubsystem : IPlugableComponent {
	string Name { get; }
	Task Start();
	Task Stop();
}
