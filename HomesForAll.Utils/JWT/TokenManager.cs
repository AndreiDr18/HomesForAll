using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace HomesForAll.Utils.JWT
{
    public class TokenManager
    {
        

        static public JwtSecurityToken CreateToken(in List<Claim> authClaims,in IConfiguration _configuration)
        {
            var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

        public static string ExtractHeaderValueJWT(in string authToken, string claimType)
        {
            string jwt;

            if (AuthenticationHeaderValue.TryParse(authToken, out var header))
                jwt = header.Parameter;
            else throw new Exception("Couldn't parse authorization token from header");


            var tokenJSON = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

            return tokenJSON.Claims.FirstOrDefault(cl => cl.Type == $"{claimType}").Value;
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public static ClaimsPrincipal? GetPrincipalFromExpiredToken(string? authToken, IConfiguration _configuration)
        {
            string jwt;
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            if (AuthenticationHeaderValue.TryParse(authToken, out var header))
                jwt = header.Parameter;
            else throw new Exception("Couldn't parse authorization token from header");

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(jwt, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
    }
}
