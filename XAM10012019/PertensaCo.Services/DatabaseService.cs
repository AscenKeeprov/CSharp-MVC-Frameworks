using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PertensaCo.Common.Extensions;
using PertensaCo.Data;
using PertensaCo.Data.Extensions;
using PertensaCo.Entities;
using PertensaCo.Entities.Enumerations;
using PertensaCo.Services.Contracts;
using static PertensaCo.Common.Constants.DBConstants;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Constants.MVCConstants;

namespace PertensaCo.Services
{
	public class DatabaseService : IDatabaseService
	{
		private readonly PertensaDbContext dbContext;
		private readonly IFileService fileService;
		private readonly ILogger<DatabaseService> logger;
		private readonly IMaterialsService materialsService;
		private readonly UserManager<User> profileService;
		private readonly RoleManager<Role> roleService;

		public DatabaseService(PertensaDbContext dbContext,
			RoleManager<Role> roleService, UserManager<User> profileService,
			IMaterialsService materialsService, IFileService fileService,
			ILogger<DatabaseService> logger)
		{
			this.dbContext = dbContext;
			this.roleService = roleService;
			this.profileService = profileService;
			this.fileService = fileService;
			this.materialsService = materialsService;
			this.logger = logger;
		}

		private async Task BuildHierarchyAsync()
		{
			logger.LogInformation("Building company hierarchy...");
			var teams = new (string Manager, string[] Workers)[]
			{
				("cmcguigan", new [] {
					"awebber", "csharpindra", "dlangton", "eobrien", "fmcintyre",
					"foleary", "kdoyle", "kgerhardt", "nboyd", "smcintyre"
				})
			};
			foreach (var team in teams)
			{
				string managerName = team.Manager;
				try
				{
					User managerProfile = await profileService.FindByNameAsync(managerName);
					if (managerProfile != null)
					{
						if (managerProfile.Employee != null)
						{
							foreach (var workerName in team.Workers)
							{
								User workerProfile = await profileService.FindByNameAsync(workerName);
								if (workerProfile != null)
								{
									if (workerProfile.Employee != null)
									{
										if (workerProfile.Employee.Manager == null)
										{
											try
											{
												workerProfile.Employee.Manager = managerProfile.Employee;
												var managerAssignTask = profileService.UpdateAsync(workerProfile);
												await managerAssignTask;
												if (managerAssignTask.Result.Errors.Any())
												{
													string errorMessage = string.Join(Environment.NewLine,
													managerAssignTask.Result.Errors.Select(e => e.Description));
													logger.LogWarning(errorMessage);
												}
												else logger.LogInformation($"'{workerName}' assigned to '{managerName}'s team.");
											}
											catch (Exception exception)
											{
												string message = $"Could not assign '{workerName}' to '{managerName}'s team."
													+ Environment.NewLine + exception.GetMessageStack();
												logger.LogWarning(message);
											}
										}
										else
										{
											string existingManagerName = workerProfile.Employee.Manager.Profile.UserName;
											logger.LogInformation($"'{workerName}' is already part of '{existingManagerName}'s team.");
										}
									}
									else
									{
										string message = $"Could not assign '{workerName}' to '{managerName}'s team."
										+ Environment.NewLine + $" '{workerName}' is not an employee.";
										logger.LogWarning(message);
									}
								}
								else
								{
									string message = $"Could not assign '{workerName}' to '{managerName}'s team."
									+ Environment.NewLine + $" Profile '{workerName}' does not exist.";
									logger.LogWarning(message);
								}
							}
						}
						else
						{
							string message = $"Unable to create a team for profile '{managerName}'."
							+ Environment.NewLine + $" '{managerName}' is not an employee.";
							logger.LogWarning(message);
						}
					}
					else
					{
						string message = $"Unable to create a team for profile '{managerName}'."
							+ Environment.NewLine + " No such profile exists.";
						logger.LogWarning(message);
					}
				}
				catch (Exception exception)
				{
					logger.LogWarning(exception.GetMessageStack());
				}
			}
			logger.LogInformation("Company hierarchy setup complete.");
		}

		private async Task GrantRolesAsync()
		{
			logger.LogInformation("Allocating duties...");
			var profilesRoles = new (string Username, ERole[] Roles)[]
			{
				("fmcintyre", new [] { ERole.CAO }),
				("nboyd", new [] { ERole.CCO }),
				("cmcguigan", new [] { ERole.CEO }),
				("foleary", new [] { ERole.CFO }),
				("eobrien", new [] { ERole.CHRO }),
				("dlangton", new [] { ERole.CINO }),
				("csharpindra", new [] { ERole.CIO }),
				("kgerhardt", new [] { ERole.COO }),
				("kdoyle", new [] { ERole.CPO }),
				("smcintyre", new [] { ERole.CRO }),
				("awebber", new [] { ERole.ITWorker }),
				("akiprov", new [] { ERole.WebUser })
			};
			foreach (var profileRecord in profilesRoles)
			{
				string username = profileRecord.Username;
				try
				{
					User profile = await profileService.FindByNameAsync(username);
					if (profile != null)
					{
						var roleNames = profileRecord.Roles.Select(r => r.ToString());
						foreach (var roleName in roleNames)
						{
							if (await roleService.RoleExistsAsync(roleName) == true)
							{
								try
								{
									var roleAssignmentTask = profileService.AddToRoleAsync(profile, roleName);
									await roleAssignmentTask;
									if (roleAssignmentTask.Result.Errors.Any(e => e.Code.Equals(UserAlreadyInRoleErrorCode)))
									{
										logger.LogInformation($"Profile '{username}' already has role '{roleName}'.");
									}
									else
									{
										var roleClaim = new Claim(RoleKey, roleName);
										await profileService.AddClaimAsync(profile, roleClaim);
										if (profile.Employee != null)
										{
											var position = await roleService.FindByNameAsync(roleName);
											profile.Employee.Position = position;
										}
										logger.LogInformation($"Role '{roleName}' granted to profile '{username}'.");
									}
								}
								catch (Exception exception)
								{
									string message = $"Could not grant role '{roleName}' to profile '{username}'."
									+ Environment.NewLine + exception.GetMessageStack();
									logger.LogWarning(message);
								}
							}
							else
							{
								string message = $"Unable to grant role '{roleName}' to profile '{username}'."
									+ " No such role exists.";
								logger.LogWarning(message);
							}
						}
					}
					else
					{
						string message = $"Unable to grant roles for profile '{username}'."
							+ Environment.NewLine + " No such profile exists.";
						logger.LogWarning(message);
					}
				}
				catch (Exception exception)
				{
					logger.LogWarning(exception.GetMessageStack());
				}
			}
			logger.LogInformation("Duty allocation finished.");
		}

		public async Task InitializeDatabaseAsync()
		{
			logger.LogInformation("Initializing database...");
			try
			{
				await dbContext.CreateDatabaseAsync();
				if (await dbContext.DatabaseExistsAsync() == false)
				{
					throw new Exception("Database does not exist.");
				}
				logger.LogInformation("Database initialization complete.");
			}
			catch (Exception exception)
			{
				throw new OperationCanceledException("Database initialization failed.", exception);
			}
		}

		public async Task SeedDatabaseAsync()
		{
			await SeedRolesAsync();
			await SeedProfilesAsync();
			await GrantRolesAsync();
			await BuildHierarchyAsync();
			await StockWarehouseAsync();
		}

		private async Task SeedRolesAsync()
		{
			logger.LogInformation("Seeding roles...");
			try
			{
				string rolesJson = await fileService.LoadTextFileContentAsync(RolesSeedFilePath);
				var rolesToSeed = JsonConvert.DeserializeObject<Role[]>(rolesJson);
				foreach (var role in rolesToSeed)
				{
					try
					{
						if (await roleService.RoleExistsAsync(role.Name) == false)
						{
							var roleCreateTask = roleService.CreateAsync(role);
							await roleCreateTask;
							if (roleCreateTask.Result.Errors.Any())
							{
								string errorMessage = string.Join(Environment.NewLine,
									roleCreateTask.Result.Errors.Select(e => e.Description));
								logger.LogWarning(errorMessage);
							}
							else logger.LogInformation($"Role '{role.Name}' created successfully.");
							if (role.Department != EDepartment.None)
							{
								var departmentClaim = new Claim(DepartmentKey, role.Department.ToString());
								await roleService.AddClaimAsync(role, departmentClaim);
							}
						}
						else logger.LogInformation($"Role '{role.Name}' already exists.");
					}
					catch (Exception exception)
					{
						string message = $"Could not create role '{role.Name}'."
							+ Environment.NewLine + exception.GetMessageStack();
						logger.LogWarning(message);
					}
				}
				logger.LogInformation("Roles seed complete.");
			}
			catch (Exception exception)
			{
				string message = "Roles seed failed."
					+ Environment.NewLine + exception.GetMessageStack();
				logger.LogWarning(message);
			}
		}

		private async Task SeedProfilesAsync()
		{
			logger.LogInformation("Seeding profiles...");
			try
			{
				string profilesJson = await fileService.LoadTextFileContentAsync(ProfilesSeedFilePath);
				var profilesToSeed = JsonConvert.DeserializeObject<User[]>(profilesJson);
				string password = "P@ssw0rd";
				foreach (var profile in profilesToSeed)
				{
					try
					{
						if (await profileService.FindByNameAsync(profile.UserName) == null)
						{
							var profileCreateTask = profileService.CreateAsync(profile, password);
							await profileCreateTask;
							if (profileCreateTask.Result.Errors.Any())
							{
								string errorMessage = string.Join(Environment.NewLine,
									profileCreateTask.Result.Errors.Select(e => e.Description));
								logger.LogWarning(errorMessage);
							}
							else logger.LogInformation($"Profile '{profile.UserName}' created successfully.");
							string profileType = string.Empty;
							if (profile.Client != null) profileType = nameof(Client);
							if (profile.Employee != null && !profile.Employee.DateRelieved.HasValue)
							{
								profileType = nameof(Employee);
								string departmentValue = profile.Employee.Department.ToString();
								var departmentClaim = new Claim(DepartmentKey, departmentValue);
								await profileService.AddClaimAsync(profile, departmentClaim);
							}
							if (!string.IsNullOrWhiteSpace(profileType))
							{
								var typeClaim = new Claim(TypeKey, profileType);
								await profileService.AddClaimAsync(profile, typeClaim);
							}
						}
						else logger.LogInformation($"Profile '{profile.UserName}' already exists.");
					}
					catch (Exception exception)
					{
						string message = $"Could not create profile '{profile.UserName}'."
							+ Environment.NewLine + exception.GetMessageStack();
						logger.LogWarning(message);
					}
				}
				logger.LogInformation("Profiles seed complete.");
			}
			catch (Exception exception)
			{
				string message = "Profiles seed failed."
					+ Environment.NewLine + exception.GetMessageStack();
				logger.LogWarning(message);
			}
		}

		private async Task StockWarehouseAsync()
		{
			logger.LogInformation("Replenishing warehouse stock...");
			try
			{
				string materialsJson = await fileService.LoadTextFileContentAsync(MaterialsSeedFilePath);
				var materialsToStock = JsonConvert.DeserializeObject<Material[]>(materialsJson);
				foreach (var material in materialsToStock)
				{
					string materialElement = material.Element.ToString();
					string materialForm = material.Form.ToString();
					string materialName = material.Element.GetDisplayName();
					try
					{
						if (material.QuantityInKg < 0)
						{
							throw new ArgumentOutOfRangeException(nameof(material.QuantityInKg));
						}
						if (material.PricePerKgInEur <= 0)
						{
							throw new ArgumentOutOfRangeException(nameof(material.PricePerKgInEur));
						}
						var availableMaterial = materialsService.GetMaterial(materialElement, materialForm);
						if (availableMaterial == null)
						{
							await materialsService.StockAsync(material);
							logger.LogInformation($"{materialName} ({materialForm}) restocked.");
						}
						else
						{
							materialsService.UpdateStock(availableMaterial, material);
							logger.LogInformation($"{materialName} ({materialForm}) inventory updated.");
						}
					}
					catch (Exception exception)
					{
						string message = $"Could not restock {materialName} ({materialForm})."
							+ Environment.NewLine + exception.GetMessageStack();
						logger.LogWarning(message);
					}
				}
				logger.LogInformation("Warehouse stock replenished.");
			}
			catch (Exception exception)
			{
				string message = "Warehouse restock failed."
					+ Environment.NewLine + exception.GetMessageStack();
				logger.LogWarning(message);
			}
		}
	}
}
