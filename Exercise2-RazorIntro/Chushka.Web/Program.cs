using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using static Chushka.Common.Constants;

namespace Chushka.Web
{
    public class Program
    {
	public static void Main(string[] args)
	{
	    IWebHost webHost = CreateWebHostBuilder(args).Build();
	    using (var serviceScope = webHost.Services.CreateScope())
	    {
		var services = serviceScope.ServiceProvider;
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		SeedRolesAsync(roleManager).Wait();
	    }
	    webHost.Run();
	}

	public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
	    WebHost.CreateDefaultBuilder(args)
		.UseStartup<Startup>();

	private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
	{
	    if (await roleManager.FindByNameAsync(AdminRoleName) == null)
	    {
		await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
	    }
	    if (await roleManager.FindByNameAsync(UserRoleName) == null)
	    {
		await roleManager.CreateAsync(new IdentityRole(UserRoleName));
	    }
	}
    }
}
