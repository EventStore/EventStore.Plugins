using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EventStore.Plugins.Licensing;
using Xunit;

namespace EventStore.Plugins.Tests.Licensing;

public class LicenseTests {
	public static (string PublicKey, string PrivateKey) CreateKeyPair() {
		using var rsa = RSA.Create(512);
		var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
		var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
		return (publicKey, privateKey);
	}

	[Fact]
	public async Task can_create_and_validate_license() {
		var (publicKey, privateKey) = CreateKeyPair();

		var license = await License.CreateAsync(publicKey, privateKey, new Dictionary<string, object>() {
			{ "foo", "bar"},
		});

		// check repeatedly because of https://github.com/dotnet/runtime/issues/43087
		Assert.True(await license.IsValidAsync(publicKey));
		Assert.True(await license.IsValidAsync(publicKey));
		Assert.True(await license.IsValidAsync(publicKey));

		Assert.Equal("bar", license.Token.Claims.First(c => c.Type == "foo").Value);
	}

	[Fact]
	public async Task detects_incorrect_public_key() {
		var (publicKey, privateKey) = CreateKeyPair();
		var (publicKey2, _) = CreateKeyPair();

		var license = await License.CreateAsync(publicKey, privateKey, new Dictionary<string, object>() {
			{ "foo", "bar"},
		});

		Assert.False(await license.IsValidAsync(publicKey2));
	}

	[Fact]
	public async Task cannot_create_with_inconsistent_keys() {
		var (publicKey, _) = CreateKeyPair();
		var (_, privateKey) = CreateKeyPair();

		var ex = await Assert.ThrowsAsync<Exception>(() =>
			License.CreateAsync(publicKey, privateKey, new Dictionary<string, object>() {
				{ "foo", "bar"},
			}));

		Assert.Equal("Token could not be validated", ex.Message);
	}
}
