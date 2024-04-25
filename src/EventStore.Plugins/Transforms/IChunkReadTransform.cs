namespace EventStore.Plugins.Transforms;

public interface IChunkReadTransform {
	ChunkDataReadStream TransformData(ChunkDataReadStream stream);
}
