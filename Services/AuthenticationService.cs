using BCrypt.Net;
using Grpc.Core;
using GrpcService.Data;
using GrpcService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
{
	public class AuthenticationService : Authentication.AuthenticationBase
	{
		private readonly AppDbContext _db;
		public AuthenticationService(AppDbContext db)
		{
			_db = db;
		}

		public override async Task<AuthenticationResponse> Authenticate(AuthenticationRequest request, ServerCallContext context)
		{
			var user = await _db.Users.FirstOrDefaultAsync(x => x.Name == request.UserName);
			if (user == null)
			{
				throw new RpcException(new Status(StatusCode.NotFound, "User Name Not Found"));
			}
			if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
			{
				throw new RpcException(new Status(StatusCode.InvalidArgument, "Password isn`t correct"));
			}

			var authenticationRes = JwtAuthenticationManager.Authenticate(user);

			return authenticationRes;
		}

		public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
		{
			if (request.UserName == string.Empty || request.Password == string.Empty)
				throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
			var user = new User
			{
				Name = request.UserName,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
				Role = request.Role,
			};

			await _db.Users.AddAsync(user);
			await _db.SaveChangesAsync();

			return await Task.FromResult(new RegisterResponse());
		}
	}
}
