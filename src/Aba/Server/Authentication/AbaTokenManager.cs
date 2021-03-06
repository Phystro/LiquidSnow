﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Thismaker.Aba.Server.Authentication
{
    public class AbaTokenManager
    {
        public AbaTokenManager(IConfiguration configuration)
        {
            configuration.Bind("AbaServer", this);
        }

        public string JwtKey { get; set; }
        public string JwtIssuer { get; set; }

        public string GenerateToken(string userId, List<Claim> claims, int expireMinutes = 20)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer=JwtIssuer, 
                IssuedAt=now, 
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHander = new JwtSecurityTokenHandler();
                var jwtToken = tokenHander.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null) return null;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidIssuer=JwtIssuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = securityKey
                };

                var principal = tokenHander.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
