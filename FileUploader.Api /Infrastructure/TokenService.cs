namespace FileUploader.Api.Infrastructure;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JWTClaims = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

public class TokenService
{
    private readonly IConfiguration configuration;

    public TokenService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public Tokens Create(User user)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JWTClaims.Name, user.Username),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);
        var refreshToken = RefreshTokenGen.Generate();

        return new Tokens { AccessToken = token, RefreshToken = refreshToken };
    }


    public ClaimsPrincipal validate(string token)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                // if cast failed or the algorithm is not HMAC-SHA256
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception e)
        {
            return null;
        }
    }



}
