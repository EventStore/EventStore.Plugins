// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

using System.Security.Cryptography;
using EventStore.Plugins.Licensing;

namespace EventStore.Plugins.Tests.Licensing;

public class KeySizeTests {
	[Theory]
	[InlineData(512)]
	[InlineData(1024)]
	[InlineData(2048)]
	[InlineData(4096)]
	public async Task can_use_key(int keySizeInBits) {
		using var rsa = RSA.Create(keySizeInBits);
		var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
		var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

		var license = await License.CreateAsync(new Dictionary<string, object> {
			{ "foo", "bar" },
		}, publicKey, privateKey);

		(await license.ValidateAsync(publicKey)).Should().BeTrue();
	}
}
