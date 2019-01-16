using System;
using System.Linq;
using System.Threading.Tasks;
using Chushka.Data;
using Chushka.Models;
using Chushka.Models.Enumerations;
using Chushka.Services.Contracts;
using Microsoft.AspNetCore.Identity;
using static Chushka.Common.Constants;

namespace Chushka.Services
{
    public class UsersService : IUsersService
    {
	private readonly ChushkaDbContext dbContext;
	private readonly RoleManager<IdentityRole> roleManager;
	private readonly SignInManager<User> signInManager;
	private readonly UserManager<User> userManager;

	public UsersService(UserManager<User> userManager, SignInManager<User> signInManager,
	    RoleManager<IdentityRole> roleManager, ChushkaDbContext dbContext)
	{
	    this.dbContext = dbContext;
	    this.roleManager = roleManager;
	    this.signInManager = signInManager;
	    this.userManager = userManager;
	}

	public async Task<IdentityResult> AddUserAsync(string username, string password, string email, string fullName)
	{
	    User user = new User()
	    {
		Email = email?.Trim(),
		FullName = fullName?.Trim(),
		UserName = username.Trim()
	    };
	    if (!dbContext.Users.Any())
	    {
		user.Role = Enum.Parse<UserRole>(AdminRoleName);
	    }
	    else user.Role = Enum.Parse<UserRole>(UserRoleName);
	    var userIdentity = await userManager.CreateAsync(user, password);
	    userIdentity = await userManager.AddToRoleAsync(user, user.Role.ToString());
	    return userIdentity;
	}

	public bool Exists(string username)
	{
	    return dbContext.Users.Any(u => u.UserName == username);
	}

	public User GetUserByUsername(string username)
	{
	    return dbContext.Users
		.SingleOrDefault(u => u.UserName == username);
	}

	public async Task<SignInResult> LogInUserAsync(string username, string password)
	{
	    var signInResult = await signInManager.PasswordSignInAsync(username, password, false, false);
	    return signInResult;
	}

	public async Task LogOutUserAsync()
	{
	    await signInManager.SignOutAsync();
	}
    }
}
