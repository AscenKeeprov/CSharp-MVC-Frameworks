using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;

namespace PertensaCo.Tests
{
	public class MockRoleManager : RoleManager<Role>
	{
		public MockRoleManager() : base(
			new Mock<IQueryableRoleStore<Role>>().Object,
			new IRoleValidator<Role>[0],
			new Mock<ILookupNormalizer>().Object,
			new Mock<IdentityErrorDescriber>().Object,
			new Mock<ILogger<RoleManager<Role>>>().Object)
		{ }

		public override IQueryable<Role> Roles => new Role[]
		{
			new Role("Manager")
			{
				Department = EDepartment.Man
			},
			new Role("Worker")
			{
				Department = EDepartment.HR
			}
		}.AsQueryable();
	}
}
