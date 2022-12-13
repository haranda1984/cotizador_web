using System.Threading.Tasks;
using HeiLiving.Quotes.Api.Entities;

namespace HeiLiving.Quotes.Api.Services
{
    public interface IAccountsService
    {
        Task<string> Authenticate(string username, string password);
        Task RegisterUser(User user);
        Task UpdateUserPassword(User user, string newPassword);
    }
}