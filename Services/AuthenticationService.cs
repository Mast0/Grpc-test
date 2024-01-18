
using Grpc.Core;

namespace GrpcService.Services
{
	public class AuthenticationService : Authentication.AuthenticationBase
	{
		public override async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, ServerCallContext context)
		{
			var authenticationRes = JwtAuthenticationManager.Authenticate(request);
			if (authenticationRes == null)
				throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid User Credentials"));

			return authenticationRes;
		}
	}
}
