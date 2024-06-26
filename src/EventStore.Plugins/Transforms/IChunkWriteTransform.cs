using System;

namespace EventStore.Plugins.Transforms;

public interface IChunkWriteTransform {
	ChunkDataWriteStream TransformData(ChunkDataWriteStream stream);
	void CompleteData(int footerSize, int alignmentSize);
	void WriteFooter(ReadOnlySpan<byte> footer, out int fileSize);
}
