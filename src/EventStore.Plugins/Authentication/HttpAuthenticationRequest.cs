using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EventStore.Plugins.Authentication;

public enum HttpAuthenticationRequestStatus {
	None,
	Error,
	NotReady,
	Unauthenticated,
	Authenticated,
}

public class HttpAuthenticationRequest : AuthenticationRequest {
	private readonly TaskCompletionSource<(HttpAuthenticationRequestStatus, ClaimsPrincipal)> _tcs;
	private readonly CancellationTokenRegistration _cancellationRegister;

	public HttpAuthenticationRequest(HttpContext context, string authToken) : this(context,
		new Dictionary<string, string> {
			["jwt"] = authToken
		}) {
	}

	public HttpAuthenticationRequest(HttpContext context, string name, string suppliedPassword) :
		this(context, new Dictionary<string, string> {
			["uid"] = name,
			["pwd"] = suppliedPassword
		}) {
	}

	public static HttpAuthenticationRequest CreateWithValidCertificate(HttpContext context, string name, X509Certificate2 clientCertificate) =>
		new(context, new Dictionary<string, string> {
			["uid"] = name,
			["client-certificate"] = clientCertificate.ExportCertificatePem(),
		});

	private HttpAuthenticationRequest(HttpContext context, IReadOnlyDictionary<string, string> tokens) : base(
		context.TraceIdentifier, tokens) {
		_tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
		_cancellationRegister = context.RequestAborted.Register(Cancel);
	}

	private void Cancel() {
		_tcs.TrySetCanceled();
		_cancellationRegister.Dispose();
	}

	public override void Unauthorized() {
		_tcs.TrySetResult((HttpAuthenticationRequestStatus.Unauthenticated, default));
	}

	public override void Authenticated(ClaimsPrincipal principal) {
		_tcs.TrySetResult((HttpAuthenticationRequestStatus.Authenticated, principal));
	}

	public override void Error() {
		_tcs.TrySetResult((HttpAuthenticationRequestStatus.Error, default));
	}

	public override void NotReady() {
		_tcs.TrySetResult((HttpAuthenticationRequestStatus.NotReady, default));
	}

	public Task<(HttpAuthenticationRequestStatus, ClaimsPrincipal)> AuthenticateAsync() => _tcs.Task;
}
