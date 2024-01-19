namespace EventStore.Plugins.Configuration;

public interface INodeSettingsProvider {
	public string GetValue(string group, string name);
}
