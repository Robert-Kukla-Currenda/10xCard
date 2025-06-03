using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TenXCards.Frontend.Services.Handlers
{
    public static class JwtValidationHelper
    {
        public static ClaimsPrincipal ValidateJwtToken(string token, string requiredIssuare, string requiredAudience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();            

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,                
                ValidateIssuer = true,
                ValidIssuer = requiredIssuare,
                ValidateAudience = true,
                ValidAudience = requiredAudience,
                ClockSkew = TimeSpan.Zero,
                SignatureValidator = (token, parameters) =>
                {
                    // Custom signature validation logic, API returns simple JWT without signature
                    var jwtToken = new JwtSecurityToken(token);
                    if (jwtToken == null || jwtToken.SignatureAlgorithm != SecurityAlgorithms.HmacSha256)
                    {
                        throw new SecurityTokenException("Invalid token signature algorithm");
                    }
                    return jwtToken;
                },
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);                
            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
