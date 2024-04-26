using System.Security.Cryptography;
using System.Threading.Tasks;
using System;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace EventStore.Plugins.Licensing;

public record License(JsonWebToken Token) {
	public async Task<bool> IsValidAsync(string publicKey) {
		var result = await ValidateTokenAsync(publicKey, Token.EncodedToken);
		return result.IsValid;
	}

	public static async Task<License> CreateAsync(
		string publicKey,
		string privateKey,
		IDictionary<string, object> claims) {

		using var rsa = RSA.Create();
		rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
		var tokenHandler = new JsonWebTokenHandler();
		var token = tokenHandler.CreateToken(new SecurityTokenDescriptor {
			Audience = "esdb",
			Issuer = "esdb",
			Expires = DateTime.UtcNow + TimeSpan.FromHours(1),
			Claims = claims,
			SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256),
		});

		var result = await ValidateTokenAsync(publicKey, token);

		if (!result.IsValid)
			throw new Exception("Token could not be validated");

		if (result.SecurityToken is not JsonWebToken jwt)
			throw new Exception("Token is not a JWT");

		return new License(jwt);
	}

	private static async Task<TokenValidationResult> ValidateTokenAsync(string publicKey, string token) {
		// not very satisfactory https://github.com/dotnet/runtime/issues/43087
		CryptoProviderFactory.Default.CacheSignatureProviders = false;

		using var rsa = RSA.Create();
		rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
		var result = await new JsonWebTokenHandler().ValidateTokenAsync(
			token,
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
