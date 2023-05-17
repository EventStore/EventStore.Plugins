using System.Security.Cryptography;

namespace EventStore.Plugins.MD5;

public interface IMD5Provider {
	/// <summary>
	///     Creates an instance of the MD5 hash algorithm implementation
	/// </summary>
	HashAlgorithm Create();
}
