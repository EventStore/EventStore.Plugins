// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Buffers;
using System.Security.Cryptography;

namespace EventStore.Plugins.Transforms;

public class ChunkDataWriteStream(Stream chunkFileStream, HashAlgorithm checksumAlgorithm) : Stream {
	public Stream ChunkFileStream => chunkFileStream;
	public HashAlgorithm ChecksumAlgorithm => checksumAlgorithm;

	public sealed override bool CanRead => false;
	public sealed override bool CanSeek => false;
	public sealed override bool CanWrite => true;
	public sealed override int Read(byte[] buffer, int offset, int count) => throw new InvalidOperationException();
	public sealed override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
		throw new InvalidOperationException();
	public sealed override void Write(byte[] buffer, int offset, int count) =>
		throw new InvalidOperationException("use WriteAsync");
	public sealed override void Flush() =>
		throw new InvalidOperationException("use FlushAsync");

	public sealed override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException();

	public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) {
		await ChunkFileStream.WriteAsync(buffer, cancellationToken);
		Checksum(buffer);
	}

	public override Task FlushAsync(CancellationToken ct) => ChunkFileStream.FlushAsync(ct);
	public override void SetLength(long value) => ChunkFileStream.SetLength(value);
	public override long Length => ChunkFileStream.Length;
	public override long Position {
		get => ChunkFileStream.Position;
		set {
			if (ChunkFileStream.Position != 0)
				throw new InvalidOperationException("Writer's position can only be moved from 0 to a higher value.");

			ReadAndChecksum(value);

			if (ChunkFileStream.Position != value)
				throw new Exception($"Writer's position ({ChunkFileStream.Position:N0}) is not at the expected position ({value:N0})");
		}
	}

	public void Checksum(ReadOnlyMemory<byte> data) {
		// HashAlgorithm.TransformBlock() doesn't support span/memory, so we need to rent a byte array from the pool
		byte[] tmp = ArrayPool<byte>.Shared.Rent(data.Length);
		try {
			data.CopyTo(tmp.AsMemory());
			ChecksumAlgorithm.TransformBlock(tmp, 0, data.Length, null, 0);
			Array.Clear(tmp, 0, data.Length);
		} finally {
			ArrayPool<byte>.Shared.Return(tmp);
		}
	}

	private void ReadAndChecksum(long count) {
		var buffer = new byte[4096];
		long toRead = count;
		while (toRead > 0) {
			int read = ChunkFileStream.Read(buffer, 0, (int)Math.Min(toRead, buffer.Length));
			if (read == 0)
				break;

			ChecksumAlgorithm.TransformBlock(buffer, 0, read, null, 0);
			toRead -= read;
		}
	}

	protected override void Dispose(bool disposing) {
		try {
			if (!disposing)
				return;

			chunkFileStream.Dispose();
		} finally {
			base.Dispose(disposing);
		}
	}
}
