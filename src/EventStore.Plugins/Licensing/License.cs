using Microsoft.IdentityModel.JsonWebTokens;

namespace EventStore.Plugins.Licensing;

public record License(JsonWebToken Token);
