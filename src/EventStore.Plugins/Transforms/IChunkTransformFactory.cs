// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Transforms;

public interface IChunkTransformFactory {
	TransformType Type { get; }

	int TransformDataPosition(int dataPosition);

	int CreateTransformHeader(Span<byte> transformHeader);

	ValueTask ReadTransformHeader(Stream stream, Memory<byte> transformHeader, CancellationToken token = default);

	IChunkTransform CreateTransform(ReadOnlySpan<byte> transformHeader);

	int TransformHeaderLength { get; }
}
