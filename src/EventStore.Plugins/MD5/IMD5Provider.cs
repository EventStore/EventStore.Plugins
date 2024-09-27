// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Security.Cryptography;

namespace EventStore.Plugins.MD5;

public interface IMD5Provider : IPlugableComponent {
	/// <summary>
	///     Creates an instance of the MD5 hash algorithm implementation
	/// </summary>
	HashAlgorithm Create();
}
