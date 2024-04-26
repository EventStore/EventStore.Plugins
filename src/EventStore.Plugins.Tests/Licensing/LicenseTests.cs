using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using EventStore.Plugins.Licensing;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace EventStore.Plugins.Tests.Licensing;

public class LicenseTests {
	[Fact]
	public async Task can_create_and_validate_license() {
		// given
		using var rsa = RSA.Create();
		var privateKey = "MIIBOgIBAAJBALYnPoy4H96Uj2i/jA/H0lF5IHfTxrlUf9P+KapIdnNXtqJzlXsJ2GN5XbLN5EU8PIodLowci47o1rbFA9UqweECAwEAAQJAO/wkfxbLd/MYXvhlWXUGb8ohxRQ6pyGKjvduJSODzmuPPXmbCIxJZdw8XzGKpL9FGp0EHgIxPJuUl3J3xPNQGQIhAO+J7N2wCZGJvTmbhWwAlw6CvuVX4qCu7a/E0Rgvdyl7AiEAwqvEjxr7ISDizreJQrJ2U+SQhgrlcngD3j6FBcYSPVMCIQCK5O3sybNyqWyIZ85ghqMQbe2k7GXjiCsYXUZlALjW2wIgTxV4xKoxW0QA3/mvyWi2qV4xWEMU82vOJns/jxjoUAECIDm9wZGsyCDQBIx3h9eBODnQeI/b+iSe6/j8A9V7vyxU";

		rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
		var tokenHandler = new JsonWebTokenHandler();

		var token = tokenHandler.CreateToken(new SecurityTokenDescriptor {
			Audience = "esdb",
			Issuer = "esdb",
			Expires = DateTime.UtcNow + TimeSpan.FromHours(1),
			Claims = new Dictionary<string, object>() {
				{ "foo", "bar"},
			},
			SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256),
		});

		// when
		var license = await License.CreateAsync(token);

		// then
		// the constructor guarantees this, but we consider the case that someone changes
		// the token using reflection.
		// check repeatedly because of https://github.com/dotnet/runtime/issues/43087
		Assert.True(await license.IsValidAsync());
		Assert.True(await license.IsValidAsync());
		Assert.True(await license.IsValidAsync());

	}

	[Fact]
	public async Task fails_to_create_invalid_license() {
		// given
		using var rsa = RSA.Create();
		var wrongPrivateKey = "MIIBOwIBAAJBALJPeVmYiRQ016TTg6/8mVBM1sO1tF3eYajn11MSLFUThg2GGFD1kvTMHQ+voWwOD5b4CBJaQOYPeS86PS5edS0CAwEAAQJBAJXi/Iz2DfSwXr0tF7ttyKqZjMbDDTUC5HEJQhWQZzezYtVR4qNc8g58994vqGft3aE9NKbrGIY11f96B/6xp/kCIQDrQEDDFOTvCIKnHkIt2w3uBqdSAdOI46EK53DKvCX6kwIhAMIJjpGl+uLTncEr9A1Ox8QIxPgILnd3ehM2fahr8uk/AiEAwKzDXgPC7TOfLpjNwxjic8znRXdRdZBZ2cBs1N78jBkCIAIsVTpwX3T25cdqFJupjDc32ezlOo//+JAKhjHCs7/FAiBUpSRSiAWQMMM/ib/RWck9/NIUCjRBRbwcVaUfPRtu3Q==";

		rsa.ImportRSAPrivateKey(Convert.FromBase64String(wrongPrivateKey), out _);
		var tokenHandler = new JsonWebTokenHandler();

		var token = tokenHandler.CreateToken(new SecurityTokenDescriptor {
			Audience = "esdb",
			Issuer = "esdb",
			Expires = DateTime.UtcNow + TimeSpan.FromHours(1),
			Claims = new Dictionary<string, object>() {
				{ "foo", "bar"},
			},
			SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256),
		});

		// when/then
		var ex = await Assert.ThrowsAsync<Exception>(async () => {
			await License.CreateAsync(token);
		});

		Assert.Equal("Token could not be validated", ex.Message);
	}

	[Fact]
	public void can_generate_key_pair() {
		using var rsa = RSA.Create(512);
		var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());
		var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
		Assert.NotEmpty(privateKey);
		Assert.NotEmpty(publicKey);
	}
}
