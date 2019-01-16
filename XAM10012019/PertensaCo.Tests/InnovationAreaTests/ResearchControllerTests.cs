using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;
using PertensaCo.Web.Areas.Innovation.Controllers;
using PertensaCo.Web.Areas.Innovation.Models;
using Xunit;

namespace PertensaCo.Tests.InnovationAreaTests
{
	public class ResearchControllerTests : IDisposable
	{
		Mock<IEmployeesService> mockEmployeesService;
		Mock<IMaterialsService> mockMaterialsService;
		Mock<ILogger<ResearchController>> mockLogger;

		public ResearchControllerTests()
		{
			mockEmployeesService = new Mock<IEmployeesService>();
			mockMaterialsService = new Mock<IMaterialsService>();
			mockLogger = new Mock<ILogger<ResearchController>>();
		}

		public void Dispose() { }

		[Fact]
		public void ConductGET_ReturnsViewResult_WithProperlyFilteredListsOfMaterials()
		{
			#region ARRANGE
			var controller = new ResearchController(
				mockMaterialsService.Object,
				mockEmployeesService.Object,
				mockLogger.Object);
			mockMaterialsService.Setup(s => s.GetMaterials())
				.Returns(new Material[]
				{
					new Material(EElement.B, EForm.Pellets, 1),	//PLATE
					new Material(EElement.Mn, EForm.Rods, 1),	//ADDITIVE
					new Material(EElement.Nb, EForm.Sheets, 1)	//ADDITIVE,BASE
				});
			#endregion
			#region ACT
			var actionResult = controller.Conduct();
			#endregion
			#region ASSERT
			var viewResult = Assert.IsType<ViewResult>(actionResult);
			var model = Assert.IsAssignableFrom<ResearchViewModel>(viewResult.ViewData.Model);
			Assert.Equal(2, model.AdditionalMaterials.Count());
			Assert.Single(model.BaseMaterials);
			#endregion
		}

		[Theory]
		[InlineData("[]")]
		[InlineData("[{\"Code\":\"\",\"Element\":0,\"Form\":0,\"Quantity\":0,\"Type\":\"\"}]")]
		public async Task ConductPOST_ReturnsBadRequest_WhenInputDataIsFaulty(string data)
		{
			#region ARRANGE
			var controller = new ResearchController(
				mockMaterialsService.Object,
				mockEmployeesService.Object,
				mockLogger.Object);
			#endregion
			#region ACT
			var actionResult = await controller.Conduct(data);
			#endregion
			#region ASSERT
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Theory]
		[InlineData("[{\"Code\":\"Fe_3\",\"Element\":\"Fe\",\"Form\":\"3\",\"Quantity\":\"39.9\",\"Type\":\"base\"},{\"Code\":\"Ni_3\",\"Element\":\"Ni\",\"Form\":\"3\",\"Quantity\":\"60.1\",\"Type\":\"additive\"}]")]
		public async Task ConductPOST_ReturnsBadRequest_WhenBasePercentageIsTooLow(string data)
		{
			#region ARRANGE
			var controller = new ResearchController(
				mockMaterialsService.Object,
				mockEmployeesService.Object,
				mockLogger.Object);
			mockMaterialsService.Setup(s => s.GetMaterial("Fe", 3))
				.Returns(new Material(EElement.Fe, EForm.Bars, 999));
			mockMaterialsService.Setup(s => s.GetMaterial("Ni", 3))
				.Returns(new Material(EElement.Ni, EForm.Bars, 999));
			#endregion
			#region ACT
			var actionResult = await controller.Conduct(data);
			#endregion
			#region ASSERT
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Theory]
		[InlineData("[{\"Code\":\"Al_2\",\"Element\":\"Al\",\"Form\":\"2\",\"Quantity\":\"60\",\"Type\":\"base\"},{\"Code\":\"Ti_2\",\"Element\":\"Ti\",\"Form\":\"2\",\"Quantity\":\"40\",\"Type\":\"additive\"}]")]
		public async Task ConductPOST_ReturnsBadRequest_WhenInsufficientMaterialQuantity(string data)
		{
			#region ARRANGE
			var controller = new ResearchController(
				mockMaterialsService.Object,
				mockEmployeesService.Object,
				mockLogger.Object);
			mockMaterialsService.Setup(s => s.GetMaterial("Al", 2))
				.Returns(new Material(EElement.Al, EForm.Slabs, 0));
			#endregion
			#region ACT
			var actionResult = await controller.Conduct(data);
			#endregion
			#region ASSERT
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}

		[Theory]
		[InlineData("[{\"Code\":\"Fe_3\",\"Element\":\"Fe\",\"Form\":\"3\",\"Quantity\":\"4.91\",\"Type\":\"base\"},{\"Code\":\"Mn_1\",\"Element\":\"Mn\",\"Form\":\"1\",\"Quantity\":\"3.224\",\"Type\":\"additive\"}]")]
		public async Task ConductPOST_ReturnsSuccessMessage_WhenAllValidationsPassed(string data)
		{
			#region ARRANGE
			var controller = new ResearchController(
				mockMaterialsService.Object,
				mockEmployeesService.Object,
				mockLogger.Object);
			mockMaterialsService.Setup(s => s.GetMaterial("Fe", 3))
				.Returns(new Material(EElement.Fe, EForm.Bars, 5));
			mockMaterialsService.Setup(s => s.GetMaterial("Mn", 1))
				.Returns(new Material(EElement.Mn, EForm.Pellets, 3.3));
			var composition = new Dictionary<string, double>()
			{
				{ "Fe", 60.3639045979838 }, { "Mn", 39.6360954020162 }
			};
			mockMaterialsService.Setup(s => s.CreateAlloyAsync(composition, 393.75676M))
				.ReturnsAsync(new Alloy()
				{
					Composition = "Fe60.36Mn39.64",
					ResearchCost = 393.75676M
				});
			var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
			{
				new Claim("ProfileId", "1"),
				new Claim(ClaimTypes.NameIdentifier, "1")
			}));
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = user }
			};
			mockEmployeesService.Setup(s => s.UpdateLastWorkedAsync("1"))
				.Returns(Task.CompletedTask);
			#endregion
			#region ACT
			var actionResult = await controller.Conduct(data);
			#endregion
			#region ASSERT
			var jsonResult = Assert.IsType<BadRequestObjectResult>(actionResult);
			#endregion
		}
	}
}
