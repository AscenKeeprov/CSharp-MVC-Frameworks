using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PertensaCo.Common.Exceptions;
using PertensaCo.Data;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services;
using Xunit;

namespace PertensaCo.Tests.LogisticsAreaTests
{
	public class MaterialsServiceTests : IDisposable
	{
		PertensaDbContext inMemoryDbContext;

		public MaterialsServiceTests()
		{
			var dbContextOptionsBuilder = new DbContextOptionsBuilder<PertensaDbContext>();
			dbContextOptionsBuilder.UseInMemoryDatabase("MockPertensaDB");
			inMemoryDbContext = new PertensaDbContext(dbContextOptionsBuilder.Options);
			SeedMockDatabase(inMemoryDbContext);
		}

		private void SeedMockDatabase(PertensaDbContext inMemoryDbContext)
		{
			inMemoryDbContext.Warehouse.AddRangeAsync(new Material[]
			{
				new Material(EElement.Ag, EForm.Bars, 100),
				new Material(EElement.Al, EForm.Slabs, 100)
			});
			inMemoryDbContext.Alloys.AddRangeAsync(new Alloy[]
			{
				new Alloy("Ag44.6Al55.4", 1234.56M)
			});
			inMemoryDbContext.SaveChanges();
		}

		public void Dispose() { }

		[Fact]
		public void CheckAvailability_ReturnsZero_WhenMaterialNotFound()
		{
			#region ARRANGE
			var materialsService = new MaterialsService(inMemoryDbContext);
			#endregion
			#region ACT
			var materialQuantity = materialsService.CheckAvailability(string.Empty, string.Empty);
			#endregion
			#region ASSERT
			Assert.Equal(0, materialQuantity);
			#endregion
		}

		[Fact]
		public async Task CreateAlloy_Throws_WhenCompositionNotUnique()
		{
			#region ARRANGE
			var materialsService = new MaterialsService(inMemoryDbContext);
			var composition = new Dictionary<string, double>()
			{
				{ "Ag", 44.6 }, { "Al", 55.4 }
			};
			#endregion
			#region ACT
			var expectedException = new DuplicateObjectException(string.Empty, string.Empty);
			try
			{
				var alloy = await materialsService.CreateAlloyAsync(composition);
			}
			catch (Exception exception)
			{
				expectedException = exception as DuplicateObjectException;
			}
			#endregion
			#region ASSERT
			Assert.IsAssignableFrom<Exception>(expectedException);
			Assert.NotNull(expectedException);
			Assert.IsType<DuplicateObjectException>(expectedException);
			#endregion
		}

		[Fact]
		public async Task CreateAlloy_RetunrsNewAlloy_WithCorrectProperties_WhenCompositionIsUnique()
		{
			#region ARRANGE
			var materialsService = new MaterialsService(inMemoryDbContext);
			var composition = new Dictionary<string, double>()
			{
				{ "Fe", 74.5 }, { "Ti", 25.5 }
			};
			decimal researchCost = 2345.6M;
			#endregion
			#region ACT
			var alloy = await materialsService.CreateAlloyAsync(composition, researchCost);
			#endregion
			#region ASSERT
			Assert.NotNull(alloy);
			Assert.Equal("Fe74.5Ti25.5", alloy.Composition);
			Assert.Equal(researchCost, alloy.ResearchCost);
			#endregion
		}

		[Fact]
		public async Task ExpendMaterials_DecreasesMaterialReservesCorrectly()
		{
			#region ARRANGE
			var targetMaterial = await inMemoryDbContext.Warehouse
				.FirstOrDefaultAsync(m => m.Element == EElement.Ag);
			var initialReserves = targetMaterial?.QuantityInKg;
			var materialsService = new MaterialsService(inMemoryDbContext);
			double quantityToExpend = 64;
			var materialsToExpend = new (string, int, double)[]
			{
				(EElement.Ag.ToString(), (int)EForm.Bars, quantityToExpend)
			};
			#endregion
			#region ACT
			materialsService.ExpendMaterials(materialsToExpend);
			var finalReserves = targetMaterial?.QuantityInKg;
			#endregion
			#region ASSERT
			Assert.NotNull(targetMaterial);
			Assert.True(finalReserves < initialReserves);
			Assert.Equal(finalReserves, initialReserves - quantityToExpend);
			#endregion
		}
	}
}
