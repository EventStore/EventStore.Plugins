using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace EventStore.Plugins.Licensing;

public sealed class License {
	public JsonWebToken Token { get; }

	private License(JsonWebToken token) {
		Token = token;
	}

	public static async Task<License> CreateAsync(string jwt) {
		var result = await ValidateTokenAsync(jwt);

		if (!result.IsValid)
			throw new Exception("Token could not be validated");

		if (result.SecurityToken is not JsonWebToken token)
			throw new Exception("Token is not a JWT");

		return new License(token);
	}

	public async Task<bool> IsValidAsync() {
		var result = await ValidateTokenAsync(Token.EncodedToken);
		return result.IsValid;
	}

	private static async Task<TokenValidationResult> ValidateTokenAsync(string jwt) {
		// not very satisfactory https://github.com/dotnet/runtime/issues/43087
		CryptoProviderFactory.Default.CacheSignatureProviders = false;

		var publicKey = "MEgCQQC2Jz6MuB/elI9ov4wPx9JReSB308a5VH/T/imqSHZzV7aic5V7CdhjeV2yzeRFPDyKHS6MHIuO6Na2xQPVKsHhAgMBAAE=";
		using var rsa = RSA.Create();
		rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
		var result = await new JsonWebTokenHandler().ValidateTokenAsync(
			jwt,
			new TokenValidationParameters {
				ValidIssuer = "esdb",
				ValidAudience = "esdb",
				IssuerSigningKey = new RsaSecurityKey(rsa),
				ValidateAudience = true,
				ValidateIssuerSigningKey = true,
				ValidateIssuer = true,
				ValidateLifetime = true,
			});
		return result;
	}
}
