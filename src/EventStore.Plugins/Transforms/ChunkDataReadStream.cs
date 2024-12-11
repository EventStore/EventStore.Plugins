// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Transforms;

public class ChunkDataReadStream(Stream chunkFileStream) : ChunkDataStream(chunkFileStream) {

	public sealed override bool CanRead => true;
	public sealed override bool CanSeek => true;
	public sealed override bool CanWrite => false;

	public override bool CanTimeout => true;

	public override int Read(Span<byte> buffer) => ChunkFileStream.Read(buffer);

	public sealed override void Write(ReadOnlySpan<byte> buffer)
		=> throw new NotSupportedException();

	public sealed override void Flush() => throw new NotSupportedException();
	public sealed override Task FlushAsync(CancellationToken cancellationToken)
		=> Task.FromException(new NotSupportedException());

	public sealed override void SetLength(long value) => throw new NotSupportedException();

	public override long Length => throw new NotSupportedException();

	// seeks need to support only `SeekOrigin.Begin`
	public override long Seek(long offset, SeekOrigin origin) {
		if (origin is not SeekOrigin.Begin)
			throw new NotSupportedException();

		return ChunkFileStream.Seek(offset, origin);
	}

	public override long Position {
		get => ChunkFileStream.Position;
		set => ChunkFileStream.Position = value;
	}

	protected override void Dispose(bool disposing) {
		try {
			if (!disposing)
				return;

			ChunkFileStream.Dispose();
		} finally {
			base.Dispose(disposing);
		}
	}
}
