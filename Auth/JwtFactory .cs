using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using HeiLiving.Quotes.Api.Models;

namespace HeiLiving.Quotes.Api.Auth
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public ClaimsIdentity GenerateClaimsIdentity(string userName, Guid id, string[] roles)
        {
            var claims = new List<Claim>
            {
                new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Id, id.ToString()),
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, rol));
            }

            //if (roles != null)
            //{
            //    if(Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.SuperAdministrator)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.SuperAdministrator));
            //    }
            //    else if(Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.Administrator)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.Administrator));
            //    }
            //    else if(Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.Agent)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.Agent));
            //    }
            //    else if(Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentErena)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentErena));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentNapoles)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentNapoles));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentAlcaza)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentAlcaza));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentFlorestaDepa)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentFlorestaDepa));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentFloresta)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentFloresta));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentCoradiso)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentCoradiso));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentLaguna)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentLaguna));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentYaxche)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentYaxche));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentKante)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentKante));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentAtoya)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentAtoya));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentVitruvia)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentVitruvia));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentXxll)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentXxll));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentKoh)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentKoh));
            //    }
            //    else if (Array.FindAll(roles, role => role.Equals(Helpers.Constants.Strings.JwtClaims.AgentGamboa)).Length > 0)
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.AgentGamboa));
            //    }
            //    else 
            //    {
            //        claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.ApiAccess));
            //    }
            //}
            //else 
            //{
            //    claims.Add(new Claim(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles, Helpers.Constants.Strings.JwtClaims.ApiAccess));
            //}

            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), claims);
        }

        public async Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity)
        {
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                identity.FindFirst(Helpers.Constants.Strings.JwtClaimIdentifiers.Id)
             };
            claims.AddRange(identity.FindAll(Helpers.Constants.Strings.JwtClaimIdentifiers.Roles));

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}