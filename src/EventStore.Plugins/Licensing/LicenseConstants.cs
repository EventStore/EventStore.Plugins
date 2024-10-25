// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Licensing;

public static class LicenseConstants {
	public const int RsaMinimumKeySizeInBits = 2048;
	public const string LicensePublicKey = "MEgCQQDGtRXIWmeJqkdpQryJdKBFVvLaMNHFkDcVXSoaDzg1ahrtCrAgwYpARAvGyFs0bcwYJZaZSt9aNwpgkAPOPQM5AgMBAAE=";
	// not actually private (here it is in the source and binaries). circumventing the license mechanism is against the ESLv2 license agreement.
	internal const string LicensePrivateKey = "MIIBPAIBAAJBAMa1FchaZ4mqR2lCvIl0oEVW8tow0cWQNxVdKhoPODVqGu0KsCDBikBEC8bIWzRtzBgllplK31o3CmCQA849AzkCAwEAAQJBALFULYpNU5UBhxUi34pzsAvxWmzpoGsFFoNUTxxOdMUExvprTltFKQ/hDAyNsc8oUg0AdBzt/jDzTce/W0WerHkCIQDq6SIeuUjWqGOG/+thcLJSj0jnNJ7NFJTPZDqaiocQDwIhANiL53qkAlWIg0uPlxARDtGI/bx5irIBn9Hed81WrnA3AiEApL4U4KkebPQwwHdwEqjfVkkIXqUnjTmW1w86jjECYX8CIQCa/Hcupdgt08j0+c6K50qN2diRXwRPpy32DZ39T38GPQIgSBL+EU/YRy6nwsqLLB+6+qtMd0s1T5kpI3l9VyNM3Uc=";
}
