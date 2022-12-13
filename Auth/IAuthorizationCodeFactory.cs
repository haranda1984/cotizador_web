using System.Threading.Tasks;

namespace HeiLiving.Quotes.Api.Auth
{
    public interface IAuthorizationCodeFactory
    {
        Task<string> GenerateAuthorizationCode(string userId);
        Task<bool> VerifyAuthorizationCode(string hash, string userId);
    }
}