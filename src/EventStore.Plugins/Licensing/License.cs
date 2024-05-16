﻿using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using static System.Convert;

namespace EventStore.Plugins.Licensing;

public record License(JsonWebToken Token) {
	public async Task<bool> IsValidAsync(string publicKey) {
		var result = await ValidateTokenAsync(publicKey, Token.EncodedToken);
		return result.IsValid;
	}

	public bool IsValid(string publicKey) =>
		IsValidAsync(publicKey).GetAwaiter().GetResult();

	public static async Task<License> CreateAsync(
		string publicKey,
		string privateKey,
		IDictionary<string, object> claims) {
		using var rsa = RSA.Create();
		rsa.ImportRSAPrivateKey(FromBase64String(privateKey), out _);
		var tokenHandler = new JsonWebTokenHandler();
		var token = tokenHandler.CreateToken(new SecurityTokenDescriptor {
			Audience = "esdb",
			Issuer = "esdb",
			Expires = DateTime.UtcNow + TimeSpan.FromHours(1),
			Claims = claims,
			SigningCredentials = new(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
		});

		var result = await ValidateTokenAsync(publicKey, token);

		if (!result.IsValid)
			throw new("Token could not be validated");

		if (result.SecurityToken is not JsonWebToken jwt)
			throw new("Token is not a JWT");

		return new(jwt);
	}

	public static License Create(string publicKey, string privateKey, IDictionary<string, object>? claims = null) =>
		CreateAsync(publicKey, privateKey, claims ?? new Dictionary<string, object>()).GetAwaiter().GetResult();

	public static License Create(byte[] publicKey, byte[] privateKey, IDictionary<string, object>? claims = null) =>
		CreateAsync(ToBase64String(publicKey), ToBase64String(privateKey), claims ?? new Dictionary<string, object>()).GetAwaiter().GetResult();

	static async Task<TokenValidationResult> ValidateTokenAsync(string publicKey, string token) {
		// not very satisfactory https://github.com/dotnet/runtime/issues/43087
		CryptoProviderFactory.Default.CacheSignatureProviders = false;

		using var rsa = RSA.Create();
		rsa.ImportRSAPublicKey(FromBase64String(publicKey), out _);
		var result = await new JsonWebTokenHandler().ValidateTokenAsync(
			token,
			new() {
				ValidIssuer = "esdb",
				ValidAudience = "esdb",
				IssuerSigningKey = new RsaSecurityKey(rsa),
				ValidateAudience = true,
				ValidateIssuerSigningKey = true,
				ValidateIssuer = true,
				ValidateLifetime = true
			});

		return result;
	}
}