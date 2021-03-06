﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PertensaCo.Entities;

namespace PertensaCo.Tests
{
	public class MockUserManager : UserManager<User>
	{
		public MockUserManager() : base(
			new Mock<IQueryableUserStore<User>>().Object,
			new Mock<IOptions<IdentityOptions>>().Object,
			new Mock<IPasswordHasher<User>>().Object,
			new IUserValidator<User>[0],
			new IPasswordValidator<User>[0],
			new Mock<ILookupNormalizer>().Object,
			new Mock<IdentityErrorDescriber>().Object,
			new Mock<IServiceProvider>().Object,
			new Mock<ILogger<UserManager<User>>>().Object)
		{ }

		public override Task<IdentityResult> CreateAsync(User user, string password)
		{
			return Task.FromResult(IdentityResult.Success);
		}

		public override Task<IdentityResult> AddToRoleAsync(User user, string role)
		{
			return Task.FromResult(IdentityResult.Success);
		}

		public override Task<string> GenerateEmailConfirmationTokenAsync(User user)
		{
			return Task.FromResult(Guid.NewGuid().ToString());
		}
	}
}
