using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Auth;
using HeiLiving.Quotes.Api.Models;

namespace HeiLiving.Quotes.Api.Helpers
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions, JsonSerializerOptions serializerOptions)
        {
            var response = new
            {
                userID = identity.Claims.Single(c => c.Type == "id").Value,
                accessToken = await jwtFactory.GenerateEncodedToken(userName, identity),
                expiresIn = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return JsonSerializer.Serialize(response, serializerOptions);
        }
    }
}