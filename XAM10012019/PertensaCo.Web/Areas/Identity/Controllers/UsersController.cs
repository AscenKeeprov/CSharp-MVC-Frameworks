using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PertensaCo.Common.Extensions;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Identity.Models;
using PertensaCo.Web.Areas.Personnel.Models;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Filters;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.MVCConstants;

namespace PertensaCo.Web.Areas.Identity.Controllers
{
	[Area("Identity")]
	public class UsersController : Controller
	{
		private readonly IEmployeesService employeeService;
		private readonly IFileService fileService;
		private readonly ILogger<UsersController> logger;
		private readonly IMapper mapper;
		private readonly IMessengerService messenger;
		private readonly UserManager<User> profileService;
		private readonly SignInManager<User> signInService;

		public UsersController(UserManager<User> profileService, SignInManager<User> signInService,
			IEmployeesService employeeService, IMapper mapper, ILogger<UsersController> logger,
			IFileService fileService, IMessengerService messenger)
		{
			this.profileService = profileService;
			this.signInService = signInService;
			this.employeeService = employeeService;
			this.mapper = mapper;
			this.fileService = fileService;
			this.messenger = messenger;
			this.logger = logger;
		}

		[Authorize]
		[HttpGet(nameof(ChangePassword))]
		public IActionResult ChangePassword()
		{
			var model = new ChangePasswordViewModel();
			return View(model);
		}

		[HttpPost(nameof(ChangePassword))]
		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
		{
			if (model.OldPassword == model.NewPassword)
			{
				ViewData[ErrorKey] = string.Format(ObjectsShouldNotMatchErrorMessage,
					model.GetPropertyDisplayName(nameof(model.OldPassword)),
					model.GetPropertyDisplayName(nameof(model.NewPassword)).ToLower());
				return View(model);
			}
			User profile = await profileService.GetUserAsync(User);
			var changePasswordResult = await profileService
				.ChangePasswordAsync(profile, model.OldPassword, model.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				ViewData[ErrorKey] = string.Format(InvalidObjectErrorMessage,
					model.GetPropertyDisplayName(nameof(model.OldPassword)));
				return View(model);
			}
			await signInService.RefreshSignInAsync(profile);
			logger.LogInformation($"'{profile.UserName}' changed their password.");
			string profileViewUrl = Url.Action(
			action: nameof(Profile),
			controller: this.ControllerName(),
			values: new { pid = profile.Id },
			protocol: Request.Scheme);
			return Redirect(profileViewUrl);
		}

		[Authorize(Policy = InformationTechnologiesAuthorizationPolicyName)]
		[HttpGet("/Profiles/" + nameof(Create))]
		public IActionResult Create()
		{
			var employees = employeeService.GetEmployees();
			var model = new ProfileCreateViewModel()
			{
				Employees = employees
				.Where(e => e.Profile == null &&
				(!e.DateRelieved.HasValue || e.DateRelieved.Value.Date > DateTime.Now.Date))
				.Select(e => new SelectListItem()
				{
					Text = e.ToString(),
					Value = e.Id.ToString()
				})
				.OrderBy(i => i.Text)
			};
			return View(model);
		}

		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		[HttpPost("/Profiles/" + nameof(Create))]
		public async Task<IActionResult> Create(ProfileCreateViewModel model)
		{
			var employees = employeeService.GetEmployees();
			model.Employees = employees
				.Where(e => e.Profile == null &&
				(!e.DateRelieved.HasValue || e.DateRelieved.Value.Date > DateTime.Now.Date))
				.Select(e => new SelectListItem()
				{
					Text = e.ToString(),
					Value = e.Id.ToString()
				}).OrderBy(i => i.Text);
			if (await profileService.FindByNameAsync(model.Alias) != null)
			{
				ViewData[ErrorKey] = $"Alias '{model.Alias}' is already in use.";
				return View(model);
			}
			if (model.DateRegistered.Date < DateTimeOffset.Now.Date)
			{
				ViewData[ErrorKey] = string.Format(DateInPastErrorMessage, "Activation date");
				return View(model);
			}
			User profile = mapper.Map<User>(model);
			profile.EmailConfirmed = true;
			var profileCreateResult = await profileService.CreateAsync(profile, model.Password);
			if (!profileCreateResult.Succeeded)
			{
				ViewData[ErrorKey] = profileCreateResult.Errors.Select(e => e.Description);
				return View(model);
			}
			var typeClaim = new Claim(TypeKey, nameof(Employee));
			await profileService.AddClaimAsync(profile, typeClaim);
			if (!string.IsNullOrWhiteSpace(model.EmployeeId))
			{
				var employee = await employeeService.FindByIdAsync(model.EmployeeId);
				if (employee == null)
				{
					ViewData[ErrorKey] = string.Format(InvalidObjectErrorMessage, nameof(Employee));
					return View(model);
				}
				profile.Employee = employee;
				var departmentClaim = new Claim(DepartmentKey, employee.Department.ToString());
				await profileService.AddClaimAsync(profile, departmentClaim);
				var roleAssignResult = await profileService.AddToRoleAsync(profile, employee.Position.Name);
				if (!roleAssignResult.Succeeded)
				{
					ViewData[ErrorKey] = roleAssignResult.Errors.Select(e => e.Description);
					return View(model);
				}
				var roleClaim = new Claim(RoleKey, employee.Position.Name);
				await profileService.AddClaimAsync(profile, roleClaim);
			}
			var infoMessages = new string[0];
			infoMessages = infoMessages.Append($"Profile '{profile.UserName}' created successfully.").ToArray();
			if (profile.Employee != null)
			{
				infoMessages = infoMessages
					.Append($"Profile '{profile.UserName}' assigned to employee {profile.Employee.ToString()}.")
					.ToArray();
			}
			ViewData[InfoKey] = infoMessages;
			logger.LogInformation($"'{User.Identity.Name}' created profile '{profile.UserName}'.");
			await employeeService.UpdateLastWorkedAsync(User.GetId());
			return View(model);
		}

		[HttpGet(nameof(Delete))]
		[Authorize(Policy = WebUsersAuthorizationPolicyName)]
		public async Task<IActionResult> Delete([FromQuery(Name = "pid")]string profileId)
		{
			User profile = await profileService.FindByIdAsync(profileId);
			if (profile == null || User.GetId() != profile.Id
				|| User.HasType(nameof(Employee))) return Forbid();
			profile.Client = null;
			profile.DateDeleted = DateTimeOffset.Now;
			var rolesToRemove = profile.Roles.Select(r => r.Role.Name);
			var roleRemovalTask = await profileService.RemoveFromRolesAsync(profile, rolesToRemove);
			if (!roleRemovalTask.Succeeded)
			{
				string message = string.Join(Environment.NewLine,
					roleRemovalTask.Errors.Select(e => e.Description));
				logger.LogError(message);
			}
			var claimsToRemove = profile.Claims
				.Select(c => new Claim(c.ClaimType, c.ClaimValue));
			var claimRemovalTask = await profileService.RemoveClaimsAsync(profile, claimsToRemove);
			if (!claimRemovalTask.Succeeded)
			{
				string message = string.Join(Environment.NewLine,
					claimRemovalTask.Errors.Select(e => e.Description));
				logger.LogError(message);
			}
			logger.LogInformation($"'{profile.UserName}' deleted their profile.");
			HttpContext.Session?.Remove(UserIdKey);
			await signInService.SignOutAsync();
			return LocalRedirect("/");
		}

		[HttpGet(nameof(ForgotPassword))]
		public IActionResult ForgotPassword()
		{
			var model = new ForgotPasswordViewModel();
			return View(model);
		}

		[HttpPost(nameof(ForgotPassword))]
		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			User profile = await profileService.FindByEmailAsync(model.Email);
			if (profile == null)
			{
				ViewData[ErrorKey] = string
					.Format(ObjectNotFoundErrorMessage, $"Profile '{model.Email}'");
			}
			else
			{
				var passwordResetToken = await profileService.GeneratePasswordResetTokenAsync(profile);
				var emailToken = profile.Email.ToBase64();
				string passwordResetUrl = Url.Action(
					action: nameof(ResetPassword),
					controller: this.ControllerName(),
					values: new { prt = passwordResetToken, emt = emailToken },
					protocol: Request.Scheme);
				string messageContent = await fileService.LoadTextFileContentAsync(ResetPasswordTemplatePath);
				messageContent = messageContent.Replace("@Username", profile.UserName);
				messageContent = messageContent.Replace("@PasswordResetUrl", passwordResetUrl);
				await messenger.SendEmailAsync("Password reset", messageContent, null, profile.Email);
				model.PasswordSent = true;
				logger.LogInformation($"Password reset for profile '{profile.UserName}'.");
			}
			return View(model);
		}

		[Authorize]
		[HttpGet(nameof(Profile))]
		public async Task<IActionResult> Profile([FromQuery(Name = "pid")]string profileId)
		{
			var userId = HttpContext.GetUserId();
			if (userId != profileId) return Forbid();
			User profile = await profileService.FindByIdAsync(profileId)
				?? await profileService.FindByNameAsync(profileId);
			ProfileViewModel model = null;
			if (profile.Employee != null) model = mapper.Map<EmployeeProfileViewModel>(profile);
			else if (profile.Client != null) model = mapper.Map<ClientProfileViewModel>(profile);
			else model = mapper.Map<ProfileViewModel>(profile);
			return View(model);
		}

		[HttpPost(nameof(Profile))]
		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Profile(ClientProfileViewModel model)
		{
			User profile = await profileService.FindByIdAsync(model.Id)
				?? await profileService.FindByNameAsync(model.Id);
			if (profile.UserName != model.Alias && await profileService.FindByNameAsync(model.Alias) != null)
			{
				ViewData[ErrorKey] = $"Alias '{model.Alias}' is already taken.";
				return View(model);
			}
			if (!string.IsNullOrWhiteSpace(model.Alias)) profile.UserName = model.Alias;
			if (!string.IsNullOrWhiteSpace(model.EmailAddress) && profile.Email != model.EmailAddress)
			{
				profile.Email = model.EmailAddress;
				profile.EmailConfirmed = false;
			}
			profile.PhoneNumber = model.PhoneNumber;
			if (profile.Client != null)
			{
				profile.Client.CompanyName = model.CompanyName;
				profile.Client.FirstName = model.FirstName;
				profile.Client.LastName = model.LastName;
				profile.Client.ShippingAddress = model.ShippingAddress;
			}
			var profileUpdateResult = await profileService.UpdateAsync(profile);
			if (!profileUpdateResult.Succeeded)
			{
				ViewData[ErrorKey] = profileUpdateResult.Errors.Select(e => e.Description);
				return View(model);
			}
			if (profile.EmailConfirmed == false) await RequestEmailVerificationAsync(profile);
			logger.LogInformation($"'{profile.UserName}' updated their profile.");
			ViewData[InfoKey] = "Profile updated.";
			return View(model);
		}

		[HttpGet(nameof(Register))]
		public IActionResult Register()
		{
			var model = new RegisterViewModel();
			return View(model);
		}

		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		[HttpPost(nameof(Register))]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (await profileService.FindByNameAsync(model.Alias) != null)
			{
				ViewData[ErrorKey] = string
					.Format(ObjectExistsErrorMessage, $"Profile '{model.Alias}'");
				return View(model);
			}
			if (await profileService.FindByEmailAsync(model.Email) != null)
			{
				ViewData[ErrorKey] = $"E-mail address '{model.Email}' is already registered.";
				return View(model);
			}
			User profile = new User()
			{
				Email = model.Email,
				UserName = model.Alias
			};
			var registerResult = await profileService.CreateAsync(profile, model.Password);
			if (!registerResult.Succeeded)
			{
				ViewData[ErrorKey] = registerResult.Errors.Select(e => e.Description);
				return View(model);
			}
			var typeClaim = new Claim(TypeKey, nameof(Client));
			await profileService.AddClaimAsync(profile, typeClaim);
			var roleAssignResult = await profileService.AddToRoleAsync(profile, ERole.WebUser.ToString());
			if (!roleAssignResult.Succeeded)
			{
				ViewData[ErrorKey] = roleAssignResult.Errors.Select(e => e.Description);
				return View(model);
			}
			var roleClaim = new Claim(RoleKey, ERole.WebUser.ToString());
			await profileService.AddClaimAsync(profile, roleClaim);
			await RequestEmailVerificationAsync(profile);
			logger.LogInformation($"Profile '{profile.UserName}' registered successfully.");
			return RedirectToAction(nameof(SignIn), this.ControllerName());
		}

		private async Task RequestEmailVerificationAsync(User profile)
		{
			string emailConfirmationToken = await profileService.GenerateEmailConfirmationTokenAsync(profile);
			string emailConfirmationUrl = Url.Action(
				action: nameof(VerifyEmail),
				controller: this.ControllerName(),
				values: new { pid = profile.Id, ect = emailConfirmationToken },
				protocol: Request.Scheme);
			string messageContent = await fileService.LoadTextFileContentAsync(ConfirmEmailTemplatePath);
			messageContent = messageContent.Replace("@Username", profile.UserName);
			messageContent = messageContent.Replace("@EmailConfirmationUrl", emailConfirmationUrl);
			await messenger.SendEmailAsync("Confirm your e-mail address", messageContent, null, profile.Email);
		}

		[HttpGet(nameof(ResetPassword))]
		public IActionResult ResetPassword([FromQuery(Name = "emt")]string emailToken)
		{
			var model = new ResetPasswordViewModel()
			{
				Email = emailToken.FromBase64()
			};
			return View(model);
		}

		[HttpPost(nameof(ResetPassword))]
		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			User profile = await profileService.FindByEmailAsync(model.Email);
			var resetPasswordResult = await profileService
				.ResetPasswordAsync(profile, model.PasswordResetToken, model.Password);
			if (!resetPasswordResult.Succeeded)
			{
				ViewData[ErrorKey] = resetPasswordResult.Errors.Select(e => e.Description);
				return View(model);
			}
			logger.LogInformation($"'{profile.UserName}' reset their password.");
			return RedirectToAction(nameof(SignIn), this.ControllerName());
		}

		[HttpGet(nameof(SignIn))]
		public async Task<IActionResult> SignIn()
		{
			if (HttpContext.User.Identity.IsAuthenticated)
			{
				return LocalRedirect("/");
			}
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
			var model = new SignInViewModel();
			return View(model);
		}

		[HttpPost(nameof(SignIn))]
		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SignIn(SignInViewModel model)
		{
			User profile = await profileService.FindByNameAsync(model.Identity)
			?? await profileService.FindByEmailAsync(model.Identity);
			if (profile == null)
			{
				ViewData[ErrorKey] = string
					.Format(ObjectNotFoundErrorMessage, $"Profile '{model.Identity}'");
				return View(model);
			}
			bool profileDeactivated = false;
			if (profile.DateDeleted.HasValue)
			{
				profileDeactivated = profileDeactivated
					|| profile.DateDeleted.Value <= DateTime.Now;
			}
			if (profile.Employee != null && profile.Employee.DateRelieved.HasValue)
			{
				profileDeactivated = profileDeactivated
					|| profile.Employee.DateRelieved.Value.Date <= DateTime.Now.Date;
			}
			if (profileDeactivated)
			{
				ViewData[ErrorKey] = $"Profile '{model.Identity}' has been deactivated.";
				return View(model);
			}
			var signInResult = await signInService.PasswordSignInAsync(profile,
				model.Password, model.RememberIdentity, lockoutOnFailure: false);
			if (!signInResult.Succeeded)
			{
				var errorMessages = new string[0];
				if (signInResult.IsNotAllowed)
				{
					errorMessages = errorMessages
						.Append($"Profile '{model.Identity}' is not allowed to sign in.").ToArray();
					if (!profile.EmailConfirmed)
					{
						errorMessages = errorMessages
							.Append("E-mail address has not been confirmed.").ToArray();
					}
					if (!profile.PhoneNumberConfirmed)
					{
						errorMessages = errorMessages
							.Append("Phone number has not been confirmed.").ToArray();
					}
				}
				else if (signInResult.IsLockedOut)
				{
					var cultureInfo = HttpContext.GetRequestCultureInfo();
					string lockoutEnd = profile.LockoutEnd.Value.ToString("F", cultureInfo);
					errorMessages = errorMessages.Append(string.Format(LockoutErrorMessage,
						$"Profile '{model.Identity}'", lockoutEnd)).ToArray();
				}
				else errorMessages = errorMessages.Append("Invalid credentials!").ToArray();
				ViewData[ErrorKey] = errorMessages;
				return View(model);
			}
			profile.DateLastLogin = DateTimeOffset.Now;
			await profileService.UpdateAsync(profile);
			logger.LogInformation($"'{profile.UserName}' signed in.");
			HttpContext.Session?.SetString(UserIdKey, profile.Id);
			return LocalRedirect("/");
		}

		[Authorize]
		[HttpGet(nameof(SignOut))]
		public async Task<IActionResult> SignOut()
		{
			string username = User.Identity.Name;
			await signInService.SignOutAsync();
			if (!string.IsNullOrWhiteSpace(username))
			{
				logger.LogInformation($"'{username}' signed out.");
			}
			HttpContext.Session?.Remove(UserIdKey);
			return LocalRedirect("/");
		}

		[HttpGet(nameof(VerifyEmail))]
		public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.ProfileId))
			{
				model.Exception = new ArgumentNullException(nameof(model.ProfileId));
				model.FailureReason = "Unknown profile.";
				return View(model);
			}
			string statusMessage = $"E-mail confirmation for profile ID: {model.ProfileId} ";
			if (string.IsNullOrWhiteSpace(model.EmailConfirmationToken))
			{
				model.Exception = new ArgumentNullException(nameof(model.EmailConfirmationToken));
				model.FailureReason = string.Format(InvalidObjectErrorMessage, "Verification link");
				logger.LogError(model.Exception, $"{statusMessage}FAILED");
				return View(model);
			}
			var profileFindTask = profileService.FindByIdAsync(model.ProfileId);
			User profile = await profileFindTask;
			if (profile == null)
			{
				model.Exception = new ArgumentNullException(nameof(profile));
				model.FailureReason = string.Format(ObjectNotFoundErrorMessage, "Profile");
				logger.LogWarning($"{statusMessage}FAILED{Environment.NewLine}{model.FailureReason}");
				return View(model);
			}
			model.ProfileName = profile.ToString();
			if (profile.EmailConfirmed == false)
			{
				var emailConfirmationResult = await profileService.ConfirmEmailAsync(profile, model.EmailConfirmationToken);
				if (!emailConfirmationResult.Succeeded)
				{
					model.Exception = new ArgumentOutOfRangeException(string
					.Join(Environment.NewLine, emailConfirmationResult.Errors.Select(e => e.Description)));
					model.FailureReason = string.Format(InvalidObjectErrorMessage, "Verification link")
					+ Environment.NewLine + "It has either expired or has been tampered with.";
					logger.LogWarning(model.Exception, $"{statusMessage}FAILED");
				}
				else logger.LogInformation($"{statusMessage}SUCCEEDED");
				return View(model);
			}
			return LocalRedirect("/");
		}
	}
}
