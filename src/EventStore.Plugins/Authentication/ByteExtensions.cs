using System;
using System.Linq;

namespace EventStore.Plugins.Authentication;

internal static class ByteExtensions {
	internal static string PEM(this byte[] bytes, string label)
	{
		var base64String = string.Join('\n', Convert.ToBase64String(bytes).Chunk(64).Select(s => new string(s)));
		return $"-----BEGIN {label}-----\n" +  base64String + "\n" + $"-----END {label}-----";
	}
}
