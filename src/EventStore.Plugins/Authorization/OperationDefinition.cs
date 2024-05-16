namespace EventStore.Plugins.Authorization;

public readonly record struct OperationDefinition(string Resource, string Action);