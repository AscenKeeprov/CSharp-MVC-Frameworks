using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using PertensaCo.Common.Extensions;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Identity.Models;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Filters;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.HTMLConstants;
using static PertensaCo.Common.Constants.MVCConstants;

namespace PertensaCo.Web.Areas.Identity.Controllers
{
	[Area("Identity")]
	[Authorize(Policy = EmployeesAuthorizationPolicyName)]
	public class RolesController : Controller
	{
		private readonly IEmployeesService employeeService;
		private readonly ILogger<RolesController> logger;
		private readonly IMapper mapper;
		private readonly UserManager<User> profileService;
		private readonly RoleManager<Role> roleService;

		public RolesController(RoleManager<Role> roleService, UserManager<User> profileService,
			IEmployeesService employeeService, IMapper mapper, ILogger<RolesController> logger)
		{
			this.roleService = roleService;
			this.profileService = profileService;
			this.employeeService = employeeService;
			this.mapper = mapper;
			this.logger = logger;
		}

		[Authorize(Policy = RoleAdministratorsAuthorizationPolicyName)]
		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult Create()
		{
			var departments = EnumExtensions.GetExtendedValues(typeof(EDepartment));
			var model = new RoleCreateViewModel()
			{
				Departments = departments
				.Where(d => d.Name != EDepartment.None.ToString())
				.Select(d => new SelectListItem()
				{
					Text = d.DisplayName,
					Value = d.Name
				}).OrderBy(i => i.Text)
			};
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[ValidateModelStateFilter]
		[HttpPost(ControllerActionRouteTemplate)]
		public async Task<IActionResult> Create(RoleCreateViewModel model)
		{
			var departments = EnumExtensions.GetExtendedValues(typeof(EDepartment));
			model.Departments = departments
				.Where(d => d.Name != EDepartment.None.ToString())
				.Select(d => new SelectListItem()
				{
					Text = d.DisplayName,
					Value = d.Name
				}).OrderBy(i => i.Text);
			if (await roleService.RoleExistsAsync(model.Name))
			{
				ViewData[ErrorKey] = string
					.Format(ObjectExistsErrorMessage, $"Role '{model.Name}'");
				return View(model);
			}
			var role = mapper.Map<Role>(model);
			var createResult = await roleService.CreateAsync(role);
			if (!createResult.Succeeded)
			{
				ViewData[ErrorKey] = createResult.Errors.Select(e => e.Description);
				return View(model);
			}
			var departmentClaim = new Claim(DepartmentKey, role.Department.ToString());
			await roleService.AddClaimAsync(role, departmentClaim);
			logger.LogInformation($"'{User.Identity.Name}' created role '{role.Name}'.");
			ViewData[InfoKey] = $"Role '{role.Name}' created successfully.";
			await employeeService.UpdateLastWorkedAsync(User.GetId());
			return View(model);
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult FilterProfilesByRole([FromQuery(Name = "rn")]string roleName)
		{
			var profiles = profileService.Users.ToHashSet();
			object eligibleProfiles = null;
			if (string.IsNullOrWhiteSpace(roleName))
			{
				eligibleProfiles = profiles.Where(p => p.UserName != User.Identity.Name
				&& (p.DateDeleted == null || p.DateDeleted.Value.Date > DateTime.Now.Date))
					.Select(p => p.UserName);
			}
			else
			{
				Role role = roleService.FindByNameAsync(roleName).Result;
				var roleArea = role.Claims.FirstOrDefault(c => c.ClaimType == DepartmentKey);
				if (roleArea == null || roleArea.ClaimValue == EDepartment.None.ToString())
				{
					eligibleProfiles = profiles.Where(p => p.Employee == null
						&& !p.Roles.Any(r => r.Role.Name == roleName)
						&& p.UserName != User.Identity.Name
						&& (p.DateDeleted == null || p.DateDeleted.Value.Date > DateTime.Now.Date))
						.Select(p => p.UserName);
				}
				else
				{
					eligibleProfiles = profiles.Where(p => p.Employee != null
						&& p.Claims.Any(c => c.ClaimValue == roleArea.ClaimValue)
						&& !p.Roles.Any(r => r.Role.Name == roleName)
						&& p.UserName != User.Identity.Name
						&& (p.DateDeleted == null || p.DateDeleted.Value.Date > DateTime.Now.Date))
						.Select(p => p.UserName);
				}
			}
			return Json(new { EligibleProfiles = eligibleProfiles });
		}

		[Authorize(Policy = RoleAdministratorsAuthorizationPolicyName)]
		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult Grant()
		{
			var roles = roleService.Roles.ToHashSet();
			var profiles = profileService.Users.ToHashSet();
			var model = new RoleGrantViewModel()
			{
				Roles = roles
				.Select(r => new SelectListItem()
				{
					Text = r.ToString(),
					Value = r.Name
				}).OrderBy(i => i.Text),
				AvailableProfiles = profiles.Where(p => p.UserName != User.Identity.Name
				&& (p.DateDeleted == null || p.DateDeleted.Value.Date > DateTime.Now.Date))
				.Select(p => new SelectListItem()
				{
					Text = $"{p.UserName} ({p.Email})",
					Value = p.UserName
				}).OrderBy(i => i.Text)
			};
			return View(model);
		}

		[ValidateAntiForgeryToken]
		[ValidateModelStateFilter]
		[HttpPost(ControllerActionRouteTemplate)]
		public async Task<IActionResult> Grant(RoleGrantViewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.RoleName)) return BadRequest(new
			{
				Message = string.Format(SelectionNullErrorMessage, "a role")
			});
			if (model.SelectedProfiles == null || !model.SelectedProfiles.Any())
			{
				return BadRequest(new
				{
					Message = string.Format(SelectionNullErrorMessage, "at least one profile")
				});
			}
			var infoMessages = new string[0];
			int grantsCount = 0;
			foreach (var username in model.SelectedProfiles)
			{
				string message = string.Empty;
				User profile = await profileService.FindByNameAsync(username);
				if (await profileService.IsInRoleAsync(profile, model.RoleName))
				{
					message = $"Profile '{username}' already has role '{model.RoleName}'.";
				}
				else
				{
					var roleGrantResult = await profileService.AddToRoleAsync(profile, model.RoleName);
					if (!roleGrantResult.Succeeded)
					{
						logger.LogError(string.Join(Environment.NewLine,
							roleGrantResult.Errors.Select(e => e.Description)));
						return BadRequest(new
						{
							Message = string.Join(HtmlNewLine, roleGrantResult.Errors.Select(e => e.Description))
						});
					}
					var roleClaim = new Claim(RoleKey, model.RoleName);
					await profileService.AddClaimAsync(profile, roleClaim);
					logger.LogInformation($"'{User.Identity.Name}' granted role '{model.RoleName}' to '{username}'.");
					message = $"Role '{model.RoleName}' granted to profile '{username}'.";
					grantsCount++;
				}
				infoMessages = infoMessages.Append(message).ToArray();
			}
			if (infoMessages.Length <= ViewMaxStatusMessageLines) return Json(new
			{
				Message = string.Join(HtmlNewLine, infoMessages)
			});
			else
			{
				string message = $"Role '{model.RoleName}' granted to {grantsCount} profile(s).";
				if (infoMessages.Length > grantsCount)
				{
					int profilesInRoleCount = infoMessages.Length - grantsCount;
					message += $"<br />{profilesInRoleCount} profiles already have role '{model.RoleName}'";
				}
				await employeeService.UpdateLastWorkedAsync(User.GetId());
				return Json(new { Message = message });
			}
		}
	}
}
