using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Personnel.Controllers;
using PertensaCo.Web.Areas.Personnel.Models;
using Xunit;

namespace PertensaCo.Tests.PersonnelAreaTests
{
	public class EmployeesControllerTests : IDisposable
	{
		Mock<IMapper> mockMapper;
		Mock<IFileService> mockFileService;
		Mock<MockUserManager> mockUserManager;
		Mock<MockRoleManager> mockRoleManager;
		Mock<IEmployeesService> mockEmployeesService;
		Mock<ILogger<EmployeesController>> mockLogger;
		EmployeesController controller;

		public EmployeesControllerTests()
		{
			mockMapper = new Mock<IMapper>();
			mockFileService = new Mock<IFileService>();
			mockUserManager = new Mock<MockUserManager>();
			mockRoleManager = new Mock<MockRoleManager>();
			mockEmployeesService = new Mock<IEmployeesService>();
			mockLogger = new Mock<ILogger<EmployeesController>>();
			controller = new EmployeesController(
				mockEmployeesService.Object,
				mockUserManager.Object,
				mockRoleManager.Object,
				mockMapper.Object,
				mockFileService.Object,
				mockLogger.Object);
		}

		public void Dispose()
		{
			controller.Dispose();
		}

		[Fact]
		public void HireGET_ReturnsViewResult_WithProperlyFilteredSelectLists()
		{
			#region ARRANGE
			var manager = new Employee()
			{
				FirstName = "Manager",
				MiddleName = "Manager",
				LastName = "Manager",
				Department = EDepartment.Adm,
				HomeAddress = "Home",
				MonthlySalaryInEUR = 2345M,
				WorkAddress = "Office"
			};
			var worker = new Employee()
			{
				FirstName = "Worker",
				MiddleName = "Worker",
				LastName = "Worker",
				Department = EDepartment.Adm,
				HomeAddress = "Home",
				MonthlySalaryInEUR = 1234M,
				WorkAddress = "Office"
			};
			var relievedEmployee = new Employee()
			{
				DateRelieved = DateTime.Now,
				FirstName = "Relieved",
				MiddleName = "Relieved",
				LastName = "Relieved",
				Department = EDepartment.None,
				HomeAddress = "Undisclosed",
				MonthlySalaryInEUR = 0M,
				WorkAddress = "Relieved"
			};
			mockEmployeesService.Setup(s => s.GetEmployees())
				.Returns(new Employee[] { manager, worker });
			mockRoleManager.Setup(s => s.Roles)
				.Returns(new Role[]
				{
					new Role("Manager") { Department = EDepartment.Man },
					new Role("Worker") { Department = EDepartment.HR }
				}.AsQueryable());
			mockUserManager.Setup(s => s.Users)
				.Returns(new User[]
				{
					new User()
					{
						UserName = "manager",
						Email = "manager@pertensa.com",
						Employee = manager
					},
					new User()
					{
						UserName = "worker",
						Email = "worker@pertensa.com",
						Employee = worker
					},
					new User()
					{
						UserName = "employee",
						Email = "user@gmail.com",
						Claims = new UserClaim[]
						{
							new UserClaim() { ClaimType = "Type", ClaimValue = nameof(Employee) }
						}
					}
				}.AsQueryable());
			#endregion
			#region ACT
			var actionResult = controller.Hire();
			#endregion
			#region ASSERT
			var viewResult = Assert.IsType<ViewResult>(actionResult);
			var model = Assert.IsAssignableFrom<EmployeeHireViewModel>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Managers.Count());
			Assert.Equal(2, model.Roles.Count());
			Assert.Single(model.Profiles);
			#endregion
		}

		private EmployeeHireViewModel validEmployeeHireViewModel = new EmployeeHireViewModel()
		{
			DateHired = DateTime.UtcNow,
			DepartmentName = EDepartment.CSR.ToString(),
			FirstName = "First",
			HomeAddress = "Home",
			LastName = "Last",
			ManagerId = "1",
			MiddleName = "Middle",
			MonthlySalaryInEUR = 1234.56M,
			RoleName = "Worker",
			WorkAddress = "Office",
			PIN = "1234567890",
		};

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenPINIsInvalid()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.PIN = string.Empty;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenPINIsNotUnique()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.PIN = "EXISTINGPIN";
			var existingEmployee = new Employee() { PIN = model.PIN };
			mockEmployeesService.Setup(s => s.FindByPin(model.PIN))
				.Returns(existingEmployee);
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Theory]
		[InlineData("", "", "")]
		[InlineData("First", "", "")]
		[InlineData("", "Middle", "")]
		[InlineData("", "", "Last")]
		[InlineData("First", "Middle", "")]
		[InlineData("", "Middle", "Last")]
		[InlineData("First", "", "Last")]
		public async Task HirePOST_ReturnsBadRequest_WhenPersonalNameIsInvalid(
			string firstName, string middleName, string LastName)
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.FirstName = firstName;
			model.MiddleName = middleName;
			model.LastName = LastName;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenDepartmentIsInvalid()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.DepartmentName = string.Empty;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenManagerIsInvalid()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.ManagerId = string.Empty;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenRoleIsInvalid()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.RoleName = string.Empty;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-0.01)]
		public async Task HirePOST_ReturnsBadRequest_WhenSalaryIsInvalid(decimal salary)
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.MonthlySalaryInEUR = salary;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenHireDateIsInvalid()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.DateHired = DateTime.Now.AddDays(-1);
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Fact]
		public async Task HirePOST_ReturnsBadRequest_WhenRelieveDateIsInvalid()
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			model.IsTemporary = true;
			model.DateHired = DateTime.Now;
			model.DateRelieved = model.DateHired;
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Theory]
		[InlineData("application/octet-stream")]
		[InlineData("audio/wav")]
		[InlineData("officedocument.wordprocessingml.document")]
		[InlineData("text/plain")]
		[InlineData("video/mpeg")]
		public async Task HirePOST_ReturnsBadRequest_WhenPortraitHasInvalidMimeType(string contentType)
		{
			#region ARRANGE
			var model = validEmployeeHireViewModel;
			var filePath = Path.GetTempFileName();
			using (var stream = File.OpenRead(filePath))
			{
				model.Portrait = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
				{
					Headers = new HeaderDictionary(),
					ContentType = contentType
				};
			}
			#endregion
			#region ACT
			var actionResult = await controller.Hire(model);
			#endregion
			#region ASSERT
			Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}
	}
}
