namespace EventStore.Plugins.Authorization;

public readonly record struct Parameter(string Name, string Value) {
	public override string ToString() => $"{Name} : {Value}";
}