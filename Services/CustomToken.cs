using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LuckyDraw.Services
{
    public class CustomToken
    {
        public static Double expires;
        public static JwtSecurityToken Maketoken(string id, string name, string phone, string privateKey)
        {
            var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss.fff tt")),
                            new Claim("id", id),
                            new Claim("name", name),
                            new Claim("phone", phone),
                           };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var exp = DateTime.UtcNow.AddMinutes(expires);

            return new JwtSecurityToken(claims: claims, expires: exp, signingCredentials: signIn);
        }
    }
}
