using Abp.Domain.Services;
using ReadIraq.Authentication.JwtBearer;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace ReadIraq.Authentication.JwtBearer
{
    public class TokenAuthManager : DomainService, ITokenAuthManager
    {
        private readonly TokenAuthConfiguration _configuration;

        public TokenAuthManager(TokenAuthConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();

            var nameIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (nameIdClaim != null)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
            }

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }

    public interface ITokenAuthManager : IDomainService
    {
        string CreateAccessToken(ClaimsIdentity identity);
    }
}
