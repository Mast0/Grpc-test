using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32.SafeHandles;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrpcService
{
	public static class JwtAuthenticationManager
	{
		public const string JWT_TOKEN_KEY = "Mast65838932731";
		private const int JWT_TOKEN_VALIDITY = 30;
		public static AuthenticationResponse Authenticate(AuthenticationRequest req)
		{
			if (req.UserName != "admin" || req.Password != "admin")
				return null;

			var jwtSecurityTikenHandler = new JwtSecurityTokenHandler();
			var tokenKey = Encoding.ASCII.GetBytes(JWT_TOKEN_KEY);
			var tokenExpiryDateTime = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY);
			var securityTokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new System.Security.Claims.ClaimsIdentity(new List<Claim>
				{
					new Claim("username", req.UserName),
					new Claim(ClaimTypes.Role, "Administrator")
				}),
				Expires = tokenExpiryDateTime,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
			};

			var securityToken = jwtSecurityTikenHandler.CreateToken(securityTokenDescriptor);
			var token = jwtSecurityTikenHandler.WriteToken(securityToken);

			return new AuthenticationResponse
			{
				AccessToken = token,
				ExpiresIn = (int)tokenExpiryDateTime.Subtract(DateTime.Now).Seconds
			};
		}
	}
}
