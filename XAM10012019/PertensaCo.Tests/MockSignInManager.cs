using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PertensaCo.Entities;

namespace PertensaCo.Tests
{
	public class MockSignInManager : SignInManager<User>
	{
		public MockSignInManager() : base(
			new Mock<MockUserManager>().Object,
			new Mock<IHttpContextAccessor>().Object,
			new Mock<IUserClaimsPrincipalFactory<User>>().Object,
			new Mock<IOptions<IdentityOptions>>().Object,
			new Mock<ILogger<SignInManager<User>>>().Object,
			new Mock<IAuthenticationSchemeProvider>().Object)
		{ }
	}
}