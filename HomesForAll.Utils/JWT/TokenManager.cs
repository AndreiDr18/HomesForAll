using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Net.Http.Headers;

namespace HomesForAll.Utils.JWT
{
    public class TokenManager
    {
        

        static public JwtSecurityToken CreateToken(List<Claim> authClaims, IConfiguration _configuration)
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

        static public string ExtractHeaderValueJWT(string authToken, string claimType)
        {
            string jwt;

            if (AuthenticationHeaderValue.TryParse(authToken, out var header))
                jwt = header.Parameter;
            else throw new Exception("Couldn't parse authorization token from header");


            var tokenJSON = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

            return tokenJSON.Claims.FirstOrDefault(cl => cl.Type == $"{claimType}").Value;
        }
    }
}
