namespace EventStore.Plugins.Transforms;

public interface IChunkTransform {
	IChunkReadTransform Read { get; }
	IChunkWriteTransform Write { get; }
}
