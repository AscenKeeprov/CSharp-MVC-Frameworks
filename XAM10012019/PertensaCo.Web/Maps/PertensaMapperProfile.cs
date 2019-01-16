using AutoMapper;
using PertensaCo.Entities;
using PertensaCo.Web.Areas.Identity.Models;
using PertensaCo.Web.Areas.Personnel.Models;
using static PertensaCo.Common.Constants.GlobalConstants;
using static PertensaCo.Common.Extensions.EnumExtensions;

namespace PertensaCo.Web.Maps
{
	public class PertensaMapperProfile : Profile
	{
		public PertensaMapperProfile()
		{
			CreateMap<EmployeeHireViewModel, Employee>()
				.ForMember(e => e.DateHired, options => options.MapFrom(vm => vm.DateHired))
				.ForMember(e => e.DateRelieved, options => options.MapFrom(vm => vm.IsTemporary ? vm.DateRelieved : null))
				.ForMember(e => e.Department, options => options.MapFrom(vm => vm.DepartmentName))
				.ForMember(e => e.FirstName, options => options.MapFrom(vm => vm.FirstName))
				.ForMember(e => e.HomeAddress, options => options.MapFrom(vm => vm.HomeAddress))
				.ForMember(e => e.LastName, options => options.MapFrom(vm => vm.LastName))
				.ForMember(e => e.MiddleName, options => options.MapFrom(vm => vm.MiddleName))
				.ForMember(e => e.PIN, options => options.MapFrom(vm => vm.PIN))
				.ForMember(e => e.MonthlySalaryInEUR, options => options.MapFrom(vm => vm.MonthlySalaryInEUR))
				.ForMember(e => e.WorkAddress, options => options.MapFrom(vm => vm.WorkAddress));

			CreateMap<ProfileCreateViewModel, User>()
				.ForMember(u => u.DateRegistered, options => options.MapFrom(vm => vm.DateRegistered))
				.ForMember(u => u.Email, options => options.MapFrom(vm => vm.EmailAddress))
				.ForMember(u => u.PhoneNumber, options => options.MapFrom(vm => vm.PhoneNumber))
				.ForMember(u => u.UserName, options => options.MapFrom(vm => vm.Alias));

			CreateMap<RoleCreateViewModel, Role>()
				.ForMember(r => r.Department, options => options.MapFrom(vm => vm.DepartmentName))
				.ForMember(r => r.DisplayName, options => options.MapFrom(vm => vm.DisplayName))
				.ForMember(r => r.Name, options => options.MapFrom(vm => vm.Name));

			CreateMap<User, ClientProfileViewModel>()
				.ForMember(vm => vm.Alias, options => options.MapFrom(u => u.UserName))
				.ForMember(vm => vm.CompanyName, options => options.MapFrom(u => u.Client.CompanyName))
				.ForMember(vm => vm.DateRegistered, options => options.MapFrom(
					u => u.DateRegistered.ToString(DateDisplayFormat)))
				.ForMember(vm => vm.EmailAddress, options => options.MapFrom(u => u.Email))
				.ForMember(vm => vm.FirstName, options => options.MapFrom(u => u.Client.FirstName))
				.ForMember(vm => vm.Id, options => options.MapFrom(u => u.Id))
				.ForMember(vm => vm.LastName, options => options.MapFrom(u => u.Client.LastName ?? u.Employee.LastName))
				.ForMember(vm => vm.PhoneNumber, options => options.MapFrom(u => u.PhoneNumber))
				.ForMember(vm => vm.ShippingAddress, options => options.MapFrom(u => u.Client.ShippingAddress));

			CreateMap<User, EmployeeProfileViewModel>()
				.ForMember(vm => vm.Alias, options => options.MapFrom(u => u.UserName))
				.ForMember(vm => vm.DateHired, options => options.MapFrom(
					u => u.Employee.DateHired.ToString(DateDisplayFormat)))
				.ForMember(vm => vm.DateRegistered, options => options.MapFrom(
					u => u.DateRegistered.ToString(DateDisplayFormat)))
				.ForMember(vm => vm.DateRelieved, options => options.MapFrom(
					u => u.Employee.DateRelieved.HasValue
					? u.Employee.DateRelieved.Value.ToString(DateDisplayFormat) : string.Empty))
				.ForMember(vm => vm.Department, options => options.MapFrom(u => u.Employee.Department.GetDisplayName()))
				.ForMember(vm => vm.EmailAddress, options => options.MapFrom(u => u.Email))
				.ForMember(vm => vm.FirstName, options => options.MapFrom(u => u.Employee.FirstName))
				.ForMember(vm => vm.HomeAddress, options => options.MapFrom(u => u.Employee.HomeAddress))
				.ForMember(vm => vm.Id, options => options.MapFrom(u => u.Id))
				.ForMember(vm => vm.LastName, options => options.MapFrom(u => u.Employee.LastName))
				.ForMember(vm => vm.Manager, options => options.MapFrom(u => u.Employee.Manager.ToString()))
				.ForMember(vm => vm.MiddleName, options => options.MapFrom(u => u.Employee.MiddleName))
				.ForMember(vm => vm.PhoneNumber, options => options.MapFrom(u => u.PhoneNumber))
				.ForMember(vm => vm.Salary, options => options.MapFrom(u => u.Employee.MonthlySalaryInEUR))
				.ForMember(vm => vm.WorkAddress, options => options.MapFrom(u => u.Employee.WorkAddress));

			CreateMap<User, ProfileViewModel>()
				.ForMember(vm => vm.Alias, options => options.MapFrom(u => u.UserName))
				.ForMember(vm => vm.DateRegistered, options => options.MapFrom(
					u => u.DateRegistered.ToString(DateDisplayFormat)))
				.ForMember(vm => vm.EmailAddress, options => options.MapFrom(u => u.Email))
				.ForMember(vm => vm.Id, options => options.MapFrom(u => u.Id))
				.ForMember(vm => vm.IsEmailConfirmed, options => options.MapFrom(u => u.EmailConfirmed))
				.ForMember(vm => vm.PhoneNumber, options => options.MapFrom(u => u.PhoneNumber));
		}
	}
}
