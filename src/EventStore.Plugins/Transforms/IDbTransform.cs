namespace EventStore.Plugins.Transforms;

public interface IDbTransform {
	string Name { get; }
	TransformType Type { get; }
	IChunkTransformFactory ChunkFactory { get; }
}
