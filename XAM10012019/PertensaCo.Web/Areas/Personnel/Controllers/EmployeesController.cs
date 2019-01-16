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
using PertensaCo.Web.Areas.Personnel.Models;
using PertensaCo.Web.Extensions;
using PertensaCo.Web.Filters;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.HTMLConstants;
using static PertensaCo.Common.Constants.HTTPConstants;
using static PertensaCo.Common.Constants.MVCConstants;

namespace PertensaCo.Web.Areas.Personnel.Controllers
{
	[Area("Personnel")]
	[Authorize(Policy = HumanResourcesAuthorizationPolicyName)]
	public class EmployeesController : Controller
	{
		private readonly IEmployeesService employeeService;
		private readonly IFileService fileService;
		private readonly ILogger<EmployeesController> logger;
		private readonly IMapper mapper;
		private readonly UserManager<User> profileService;
		private readonly RoleManager<Role> roleService;

		public EmployeesController(IEmployeesService employeeService, UserManager<User> profileService,
			RoleManager<Role> roleService, IMapper mapper,
			IFileService fileService, ILogger<EmployeesController> logger)
		{
			this.employeeService = employeeService;
			this.roleService = roleService;
			this.profileService = profileService;
			this.mapper = mapper;
			this.fileService = fileService;
			this.logger = logger;
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult FilterEmployeesRolesByDepartment([FromQuery(Name = "dn")]string departmentName)
		{
			var employees = employeeService.GetEmployees();
			var roles = roleService.Roles.ToHashSet();
			object employeeIds = null;
			object roleNames = null;
			if (string.IsNullOrWhiteSpace(departmentName))
			{
				employeeIds = employees
					.Where(e => !e.DateRelieved.HasValue)
					.Select(e => e.Id.ToString());
				roleNames = roles
					.Where(r => r.Department != EDepartment.None)
					.Select(r => r.Name);
			}
			else
			{
				employeeIds = employees
				.Where(e => !e.DateRelieved.HasValue && e.Department.ToString() == departmentName)
				.Select(e => e.Id.ToString());
				roleNames = roles
					.Where(r => r.Department != EDepartment.None && r.Department.ToString() == departmentName)
					.Select(r => r.Name);
			}
			return Json(new { EmployeeIds = employeeIds, RoleNames = roleNames });
		}

		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult FilterProfilesWithoutEmployee()
		{
			var profiles = profileService.Users.ToHashSet();
			var profileIds = profiles
				.Where(p => p.Employee == null && p.Claims.Any(
					c => c.ClaimType == TypeKey && c.ClaimValue == nameof(Employee)))
				.Select(p => p.Id);
			return Json(new { ProfileIds = profileIds });
		}


		[HttpGet(ControllerActionRouteTemplate)]
		public IActionResult Hire()
		{
			var departments = EnumExtensions.GetExtendedValues(typeof(EDepartment));
			var employees = employeeService.GetEmployees();
			var roles = roleService.Roles.ToHashSet();
			var profiles = profileService.Users.ToHashSet();
			var model = new EmployeeHireViewModel()
			{
				Departments = departments.Where(d => d.Name != EDepartment.None.ToString())
				.Select(d => new SelectListItem()
				{
					Text = d.DisplayName,
					Value = d.Name
				}).OrderBy(i => i.Text),
				Managers = employees.Where(e => !e.DateRelieved.HasValue)
				.Select(e => new SelectListItem()
				{
					Text = e.ToString(),
					Value = e.Id.ToString()
				}).OrderBy(i => i.Text),
				Profiles = profiles.Where(p => p.Employee == null && p.Claims.Any(
					c => c.ClaimType == TypeKey && c.ClaimValue == nameof(Employee)))
				.Select(p => new SelectListItem()
				{
					Text = $"{p.UserName} ({p.Email})",
					Value = p.Id
				}).OrderBy(i => i.Text),
				Roles = roles.Where(r => r.Department != EDepartment.None)
				.Select(r => new SelectListItem()
				{
					Text = r.ToString(),
					Value = r.Name
				}).OrderBy(i => i.Text)
			};
			return View(model);
		}

		[ValidateModelStateFilter]
		[ValidateAntiForgeryToken]
		[HttpPost(ControllerActionRouteTemplate)]
		public async Task<IActionResult> Hire(EmployeeHireViewModel model)
		{
			if (string.IsNullOrWhiteSpace(model.PIN)) return BadRequest(new
			{
				Message = string.Format(StringBlankErrorMessage,
				model.GetPropertyDisplayName(nameof(model.PIN)))
			});
			if (employeeService.FindByPin(model.PIN) != null) return BadRequest(new
			{
				Message = string.Format(ObjectExistsErrorMessage,
				"A record with the same identification number")
			});
			if (string.IsNullOrWhiteSpace(model.FirstName)
				|| string.IsNullOrWhiteSpace(model.MiddleName)
				|| string.IsNullOrWhiteSpace(model.LastName)) return BadRequest(new
				{
					Message = string.Format(StringBlankErrorMessage, "A name")
				});
			if (string.IsNullOrWhiteSpace(model.DepartmentName)) return BadRequest(new
			{
				Message = string.Format(SelectionNullErrorMessage, "a department")
			});
			if (string.IsNullOrWhiteSpace(model.ManagerId)) return BadRequest(new
			{
				Message = string.Format(SelectionNullErrorMessage, "a manager")
			});
			if (string.IsNullOrWhiteSpace(model.RoleName)) return BadRequest(new
			{
				Message = string.Format(SelectionNullErrorMessage, "a role")
			});
			if (model.MonthlySalaryInEUR <= 0) return BadRequest(new
			{
				Message = string.Format(InvalidObjectErrorMessage, "Salary")
			});
			string dateHiredDisplayName = model.GetPropertyDisplayName(nameof(model.DateHired));
			if (model.DateHired.Date < DateTimeOffset.Now.Date) return BadRequest(new
			{
				Message = string.Format(DateInPastErrorMessage, dateHiredDisplayName)
			});
			if (model.IsTemporary && model.DateRelieved.Value.Date <= model.DateHired.Date)
			{
				string dateRelievedDisplayName = model.GetPropertyDisplayName(nameof(model.DateRelieved));
				return BadRequest(new
				{
					Message = $"{dateRelievedDisplayName} must be after {dateHiredDisplayName.ToLower()}!"
				});
			}
			string portraitMimeType = string.Empty;
			if (model.Portrait != null)
			{
				string propertyDisplayName = model.GetPropertyDisplayName(nameof(model.Portrait));
				portraitMimeType = await fileService.GetMimeTypeAsync(model.Portrait);
				if (string.IsNullOrWhiteSpace(portraitMimeType)) return BadRequest(new
				{
					Message = $"{propertyDisplayName} is corrupted."
				});
				if (!portraitMimeType.StartsWith(ImageMimeTypePrefix)) return BadRequest(new
				{
					Message = $"{propertyDisplayName} must be an image file!"
				});
			}
			var employee = mapper.Map<Employee>(model);
			employee.Manager = await employeeService.FindByIdAsync(model.ManagerId);
			var role = await roleService.FindByNameAsync(model.RoleName);
			employee.Position = role;
			if (!string.IsNullOrWhiteSpace(model.ProfileId))
			{
				var profile = await profileService.FindByIdAsync(model.ProfileId);
				if (profile.Employee != null) return BadRequest(new
				{
					Message = $"Profile '{profile.UserName}' is already in use by another employee."
				});
				employee.Profile = profile;
				if (!employee.Profile.Claims.Any(c => c.ClaimType == TypeKey))
				{
					var typeClaim = new Claim(TypeKey, nameof(Employee));
					await profileService.AddClaimAsync(employee.Profile, typeClaim);
				}
				if (!employee.Profile.Claims.Any(c => c.ClaimType == DepartmentKey))
				{
					var departmentClaim = new Claim(DepartmentKey, employee.Department.ToString());
					await profileService.AddClaimAsync(employee.Profile, departmentClaim);
				}
				var roleAssignTask = await profileService.AddToRoleAsync(employee.Profile, model.RoleName);
				if (!roleAssignTask.Succeeded) return BadRequest(new
				{
					Message = string.Join(HtmlNewLine, roleAssignTask.Errors.Select(e => e.Description))
				});
				if (!employee.Profile.Claims.Any(c => c.ClaimValue == model.RoleName))
				{
					var roleClaim = new Claim(RoleKey, model.RoleName);
					await profileService.AddClaimAsync(employee.Profile, roleClaim);
				}
				employee.Profile.DateDeleted = employee.DateRelieved;
			}
			try
			{
				await employeeService.HireAsync(employee);
				if (!string.IsNullOrWhiteSpace(portraitMimeType))
				{
					string portraitFileType = portraitMimeType.Replace(ImageMimeTypePrefix, string.Empty);
					string portraitFileName = $"portrait.{portraitFileType}";
					string profileDirectoryName = employee.PIN.ToBase64();
					string portraitFilePath = fileService.BuildPath(new string[]
					{
						PersonnelDataRoot, profileDirectoryName, portraitFileName
					}, trimmed: true);
					await fileService.SaveFileAsync(model.Portrait, portraitFilePath);
				}
				logger.LogInformation($"'{User.Identity.Name}' hired a new employee - {employee.ToString()}.");
				await employeeService.UpdateLastWorkedAsync(User.GetId());
				return Json(new { Message = $"{employee.ToString()} hired." });
			}
			catch (Exception exception)
			{
				logger.LogError(exception.GetMessageStack());
				return BadRequest(new { Message = exception.GetMessageStack(HtmlNewLine) });
			}
		}
	}
}
