// Copyright (c) Event Store Ltd and/or licensed to Event Store Ltd under one or more agreements.
// Event Store Ltd licenses this file to you under the Event Store License v2 (see LICENSE.md).

namespace EventStore.Plugins.Licensing;

// Allows plugins to access the current license, get updates to it, and reject a license
// if it is missing entitlements
public interface ILicenseService {
	// For checking that the license service itself is authentic
	License SelfLicense { get; }

	License? CurrentLicense { get; }

	// The current license and updates to it
	IObservable<License> Licenses { get; }

	void RejectLicense(Exception ex);
}
