using System.Net;
using Chushka.Data;
using Chushka.Models;
using Chushka.Services;
using Chushka.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Chushka.Common.Constants;

namespace Chushka.Web
{
    public class Startup
    {
	public Startup(IConfiguration configuration)
	{
	    Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services)
	{
	    services.AddDbContext<ChushkaDbContext>();
	    services.AddIdentity<User, IdentityRole>(options =>
	    {
		options.Password.RequireDigit = false;
		options.Password.RequireLowercase = false;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequireUppercase = false;
		options.Password.RequiredLength = 1;
		options.Password.RequiredUniqueChars = 1;
		options.SignIn.RequireConfirmedEmail = false;
	    })
		.AddEntityFrameworkStores<ChushkaDbContext>()
		.AddDefaultTokenProviders()
		.AddDefaultUI();
	    services.AddScoped<IOrdersService, OrdersService>();
	    services.AddScoped<IProductsService, ProductsService>();
	    services.AddScoped<IUsersService, UsersService>();
	    services.Configure<CookiePolicyOptions>(options =>
	    {
		options.CheckConsentNeeded = context => false;
		options.MinimumSameSitePolicy = SameSiteMode.Lax;
	    });
	    services.AddMvc()
		.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
	}

	public void Configure(IApplicationBuilder app, IHostingEnvironment env)
	{
	    if (env.IsDevelopment())
	    {
		app.UseDeveloperExceptionPage();
		app.UseDatabaseErrorPage();
		app.UseStatusCodePages();
	    }
	    else
	    {
		app.UseExceptionHandler(ErrorHandlingPath);
		app.UseHsts();
	    }

	    app.UseHttpsRedirection();
	    app.UseRewriter(new RewriteOptions()
		.AddRedirect(@"Identity/Account/([^?]*).*", "Users/$1", (int)HttpStatusCode.Redirect));
	    app.UseStaticFiles();
	    app.UseAuthentication();
	    app.UseCookiePolicy();
	    app.UseMvc(routes =>
	    {
		routes.MapRoute(
		    name: "default",
		    template: "{controller=Home}/{action=Index}/{id?}");
	    });
	}
    }
}
