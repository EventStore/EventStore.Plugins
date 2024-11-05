// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System;

namespace EventStore.Plugins.Transforms;

public interface IChunkWriteTransform {
	ChunkDataWriteStream TransformData(ChunkDataWriteStream stream);
	ValueTask CompleteData(int footerSize, int alignmentSize, CancellationToken cancellationToken = default);
	ValueTask<int> WriteFooter(ReadOnlyMemory<byte> footer, CancellationToken cancellationToken = default);
}
