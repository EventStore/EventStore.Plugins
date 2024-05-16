using System.Security.Cryptography;
using EventStore.Plugins.Licensing;

namespace EventStore.Plugins.Tests.Licensing;

public class LicenseTests {
	static (string PublicKey, string PrivateKey) CreateKeyPair() {
		using var rsa = RSA.Create(1024); // was failing with 512?!?
		var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
		var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
		return (publicKey, privateKey);
	}

	[Fact]
	public async Task can_create_and_validate_license() {
		var (publicKey, privateKey) = CreateKeyPair();

		var license = await License.CreateAsync(publicKey, privateKey, new Dictionary<string, object> {
			{ "foo", "bar" }
		});

		// check repeatedly because of https://github.com/dotnet/runtime/issues/43087
		(await license.IsValidAsync(publicKey)).Should().BeTrue();
		(await license.IsValidAsync(publicKey)).Should().BeTrue();
		(await license.IsValidAsync(publicKey)).Should().BeTrue();

		license.Token.Claims.First(c => c.Type == "foo").Value.Should().Be("bar");
	}

	[Fact]
	public async Task detects_incorrect_public_key() {
		var (publicKey, privateKey) = CreateKeyPair();
		var (publicKey2, _) = CreateKeyPair();

		var license = await License.CreateAsync(publicKey, privateKey, new Dictionary<string, object> {
			{ "foo", "bar" }
		});

		(await license.IsValidAsync(publicKey2)).Should().BeFalse();
	}

	[Fact]
	public async Task cannot_create_with_inconsistent_keys() {
		var (publicKey, _) = CreateKeyPair();
		var (_, privateKey) = CreateKeyPair();

		Func<Task> act = () => License.CreateAsync(publicKey, privateKey, new Dictionary<string, object> {
			{ "foo", "bar" }
		});

		await act.Should().ThrowAsync<Exception>().WithMessage("Token could not be validated");
	}
}