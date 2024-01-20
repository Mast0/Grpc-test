using GrpcService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrpcService
{
	public static class JwtAuthenticationManager
	{
		public const string JWT_TOKEN_KEY = "qO*L>m~w2g8tAz$`u!r#D+vF;6cH0ZJxRnPbUydkL9:i'W,7eBhY3Q1{XV]C4}f.TK^ISl5Eo%N&";
		private const int JWT_TOKEN_VALIDITY = 30;
		public static AuthenticationResponse Authenticate(User user)
		{
			string userRole;
			if (user.Role == 0)
			{
				userRole = "Administrator";
			}
            else if (user.Role == 1)
            {
				userRole = "User";
            }
			else
			{
				userRole = string.Empty;
			}

			List<Claim> claims = new List<Claim> 
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Role, userRole)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_TOKEN_KEY));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var tokenExpiryDateTime = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: tokenExpiryDateTime,
				signingCredentials: creds
				);

			var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

			return new AuthenticationResponse
			{
				AccessToken = jwtToken,
				ExpiresIn = (int)tokenExpiryDateTime.Subtract(DateTime.Now).Seconds
			};
		}
	}
}
