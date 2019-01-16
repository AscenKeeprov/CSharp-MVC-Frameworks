using System.Threading.Tasks;
using Chushka.Models;
using Microsoft.AspNetCore.Identity;

namespace Chushka.Services.Contracts
{
    public interface IUsersService
    {
	Task<IdentityResult> AddUserAsync(string username, string password, string email, string fullName);
	bool Exists(string username);
	User GetUserByUsername(string username);
	Task<SignInResult> LogInUserAsync(string username, string password);
	Task LogOutUserAsync();
    }
}
