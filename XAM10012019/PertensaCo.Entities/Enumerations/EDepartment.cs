using System.ComponentModel;

namespace PertensaCo.Entities.Enumerations
{
	public enum EDepartment : byte
	{
		[DisplayName("Administration")]
		Adm = 2,
		[DisplayName("Customer Support & Relationships")]
		CSR = 10,
		[DisplayName("Finance & Business Intelligence")]
		FBI = 3,
		[DisplayName("Human Resources")]
		HR = 4,
		[DisplayName("Information Technologies")]
		IT = 9,
		[DisplayName("Logistics & Procurement")]
		LnP = 6,
		[DisplayName("Management")]
		Man = 100,
		None = 0,
		[DisplayName("Operations")]
		Ops = 7,
		[DisplayName("Research & Development")]
		RnD = 5,
		[DisplayName("Sales & Marketing")]
		SnM = 8
	}
}
