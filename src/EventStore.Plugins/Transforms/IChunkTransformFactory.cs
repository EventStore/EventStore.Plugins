using System;
using System.IO;

namespace EventStore.Plugins.Transforms;

public interface IChunkTransformFactory {
	TransformType Type { get; }
	int TransformDataPosition(int dataPosition);
	ReadOnlyMemory<byte> CreateTransformHeader();
	ReadOnlyMemory<byte> ReadTransformHeader(Stream stream);
	IChunkTransform CreateTransform(ReadOnlyMemory<byte> transformHeader);
}
