using System;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PertensaCo.Data;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services;
using PertensaCo.Services.Contracts;
using PertensaCo.Services.Extensions;
using PertensaCo.Web.Areas.Identity.Factories;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Maps;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.HTMLConstants;
using static PertensaCo.Common.Constants.HTTPConstants;
using static PertensaCo.Common.Constants.MVCConstants;
using static PertensaCo.Common.Constants.UserConstants;

namespace PertensaCo.Web
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
			services.AddDbContext<PertensaDbContext>();
			services.AddAutoMapper(configuration =>
			{
				configuration.AddProfile<PertensaMapperProfile>();
			});
			services.AddIdentity<User, Role>(options =>
			{
				options.ClaimsIdentity.RoleClaimType = RoleKey;
				options.ClaimsIdentity.UserIdClaimType = ProfileIdKey;
				options.ClaimsIdentity.UserNameClaimType = AliasKey;
				options.Lockout.AllowedForNewUsers = LockoutEnabledWhenNew;
				options.Password.RequiredLength = PasswordMinLength;
				options.Password.RequireDigit = PasswordHasDigit;
				options.Password.RequireLowercase = PasswordHasLowercase;
				options.Password.RequireNonAlphanumeric = PasswordHasSymbol;
				options.Password.RequireUppercase = PasswordHasUppercase;
				options.SignIn.RequireConfirmedEmail = EmailRequireConfirmed;
				options.SignIn.RequireConfirmedPhoneNumber = PhoneNumberRequireConfirmed;
				options.User.AllowedUserNameCharacters = UsernameAllowedCharacters;
				options.User.RequireUniqueEmail = EmailIsUnique;
			})
			.AddEntityFrameworkStores<PertensaDbContext>()
			.AddDefaultTokenProviders();
			services.ConfigureApplicationCookie(options =>
			{
				options.AccessDeniedPath = ForbiddenViewRoute;
				options.Cookie.Name = ApplicationCookieName;
				options.Cookie.Path = RootCookiePath;
				options.LoginPath = LoginViewRoute;
				options.LogoutPath = LogoutViewRoute;
			});
			services.Configure<CookiePolicyOptions>(options =>
			{
				options.CheckConsentNeeded = context => ConsentCookieIsRequired;
				options.ConsentCookie.Name = ConsentCookieName;
				options.MinimumSameSitePolicy = SameSiteMode.None;
				options.HttpOnly = HttpOnlyPolicy.None;
			});
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.AccessDeniedPath = ForbiddenViewRoute;
				options.Cookie.Name = AuthenticationCookieName;
				options.Cookie.HttpOnly = AuthenticationCookieIsHttpOnly;
				options.Cookie.IsEssential = AuthenticationCookieIsRequired;
				options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
				options.LoginPath = LoginViewRoute;
				options.LogoutPath = LogoutViewRoute;
			});
			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.Cookie.HttpOnly = SessionCookieIsHttpOnly;
				options.Cookie.IsEssential = SessionCookieIsRequired;
				options.Cookie.Name = SessionCookieName;
				options.Cookie.Path = RootCookiePath;
				options.IdleTimeout = TimeSpan.FromSeconds(SessionTimeoutInSeconds);
			});
			services.AddAntiforgery(options =>
			{
				options.Cookie.Name = AntiforgeryCookieName;
				options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
				options.FormFieldName = AntiforgeryTokenFieldName;
			});
			services.AddScoped<IDatabaseService, DatabaseService>();
			services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsFactory>();
			services.AddScoped<IEmployeesService, EmployeesService>();
			services.AddScoped<IMaterialsService, MaterialsService>();
			services.AddScoped<IFileService, FileService>();
			services.AddMessenger(options =>
			{
				options.DefaultMailboxName = CompanyName;
				options.SmtpServerName = GoogleSmtpServerName;
				options.SmtpServerPassword = GoogleSmtpServerPassword;
				options.SmtpServerPort = GoogleSmtpServerTLSPort;
				options.SmtpServerSecurePort = GoogleSmtpServerSSLPort;
				options.SmtpServerUsername = GoogleSmtpServerPrincipal;
			});
			services.AddMvc(options =>
			{
				options.MaxModelValidationErrors = ModelMaxValidationErrors;
			})
			.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddAuthorization(options =>
			{
				options.AddPolicy(EmployeesAuthorizationPolicyName, policy
					=> policy.RequireClaim(TypeKey, nameof(Employee)));
				options.AddPolicy(HumanResourcesAuthorizationPolicyName, policy
					=> policy.RequireClaim(DepartmentKey, EDepartment.HR.ToString()));
				options.AddPolicy(InformationTechnologiesAuthorizationPolicyName, policy
					=> policy.RequireClaim(DepartmentKey, EDepartment.IT.ToString()));
				options.AddPolicy(LogisticsAuthorizationPolicyName, policy
					=> policy.RequireClaim(DepartmentKey, EDepartment.LnP.ToString()));
				options.AddPolicy(ScientistsAuthorizationPolicyName, policy
					=> policy.RequireClaim(DepartmentKey, EDepartment.RnD.ToString()));
				options.AddPolicy(RoleAdministratorsAuthorizationPolicyName, policy
					=> policy.RequireAssertion(context
					=> context.User.HasClaim(c => c.Type == DepartmentKey
					&& c.Value == EDepartment.HR.ToString()
					|| c.Value == EDepartment.IT.ToString())));
				options.AddPolicy(WebUsersAuthorizationPolicyName, policy
					=> policy.RequireRole(ERole.WebUser.ToString()));
			});
		}

		public void Configure(IApplicationBuilder application, IHostingEnvironment environment)
		{
			if (environment.IsDevelopment())
			{
				application.UseDeveloperExceptionPage();
				application.UseDatabaseErrorPage();
			}
			else if (environment.IsProduction() || environment.IsStaging())
			{
				application.UseClientExceptionPage(ErrorViewRoute);
				application.UseHsts();
			}
			application.UseStatusCodePages();
			application.UseHttpsRedirection();
			application.UseStaticFiles();
			application.UseCookiePolicy();
			application.UseAuthentication();
			application.UseSession();
			application.UseRequestLocalization();
			application.UseMvc();
		}
	}
}