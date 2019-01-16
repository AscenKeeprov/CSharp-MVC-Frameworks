using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Moq;
using PertensaCo.Common.Extensions;
using PertensaCo.Entities;
using PertensaCo.Entities.Attributes;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Identity.Controllers;
using PertensaCo.Web.Areas.Identity.Models;
using PertensaCo.Web.Areas.Innovation.Controllers;
using PertensaCo.Web.Areas.Innovation.Models;
using Xunit;

namespace PertensaCo.Tests.IdentityAreaTests
{
	public class UsersControllerTests : IDisposable
	{
		Mock<IMapper> mockMapper;
		Mock<IFileService> mockFileService;
		Mock<MockUserManager> mockUserManager;
		Mock<MockSignInManager> mockSignInManager;
		Mock<ILogger<UsersController>> mockLogger;
		Mock<IMessengerService> mockMessengerService;
		Mock<IEmployeesService> mockEmployeesService;

		UsersController controller;

		public UsersControllerTests()
		{
			mockMapper = new Mock<IMapper>();
			mockFileService = new Mock<IFileService>();
			mockUserManager = new Mock<MockUserManager>();
			mockSignInManager = new Mock<MockSignInManager>();
			mockEmployeesService = new Mock<IEmployeesService>();
			mockMessengerService = new Mock<IMessengerService>();
			mockLogger = new Mock<ILogger<UsersController>>();
			controller = new UsersController(
				mockUserManager.Object,
				mockSignInManager.Object,
				mockEmployeesService.Object,
				mockMapper.Object,
				mockLogger.Object,
				mockFileService.Object,
				mockMessengerService.Object);
		}

		public void Dispose()
		{
			controller.Dispose();
		}

		[Fact]
		public void ChangePasswordGET_ReturnsViewResult_WithEmptyPasswordFields()
		{
			#region ARRANGE
			#endregion
			#region ACT
			var actionResult = controller.ChangePassword();
			#endregion
			#region ASSERT
			var viewResult = Assert.IsType<ViewResult>(actionResult);
			var model = Assert.IsAssignableFrom<ChangePasswordViewModel>(viewResult.ViewData.Model);
			Assert.Null(model.OldPassword);
			Assert.Null(model.NewPassword);
			Assert.Null(model.RepeatNewPassword);
			#endregion
		}

		[Fact]
		public async Task ChangePasswordPOST_ReturnsError_WhenOldAndNewPasswordAreTheSame()
		{
			#region ARRANGE
			string password = "P@ssw0rd";
			var model = new ChangePasswordViewModel()
			{
				OldPassword = password,
				NewPassword = password
			};
			#endregion
			#region ACT
			var actionResult = await controller.ChangePasswordAsync(model);
			#endregion
			#region ASSERT
			var viewResult = Assert.IsType<ViewResult>(actionResult);
			Assert.IsAssignableFrom<ChangePasswordViewModel>(viewResult.ViewData.Model);
			Assert.True(viewResult.ViewData.ContainsKey("Error"));
			#endregion
		}

		[Fact]
		public void CreateGET_ReturnsViewResult_WithProperlyFilteredListOfEmployees()
		{
			#region ARRANGE
			mockEmployeesService.Setup(s => s.GetEmployees())
				.Returns(new Employee[]
				{
					new Employee() { DateRelieved = DateTime.Now.AddDays(1), Profile = null },
					new Employee() { Profile = null },
					new Employee() { DateRelieved = DateTime.Now.AddDays(-1), Profile = null },
				});
			#endregion
			#region ACT
			var actionResult = controller.Create();
			#endregion
			#region ASSERT
			var viewResult = Assert.IsType<ViewResult>(actionResult);
			var model = Assert.IsAssignableFrom<ProfileCreateViewModel>(viewResult.ViewData.Model);
			Assert.Equal(2, model.Employees.Count());
			#endregion
		}

		[Fact]
		public void UserCanUpdateEmail()
		{
			User profile = new User()
			{
				Email = "initial@email.com"
			};
			string newEmail = "updated@email.com";
			profile.Email = newEmail;
			Assert.Equal(newEmail, profile.Email);
		}

		[Fact]
		public void UserHasNeverLoggedInWhenNew()
		{
			User profile = new User();
			Assert.Null(profile.DateLastLogin);
		}
	}
}
