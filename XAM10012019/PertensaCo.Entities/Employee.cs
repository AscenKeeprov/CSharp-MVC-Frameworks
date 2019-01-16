using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PertensaCo.Entities.Enumerations;

namespace PertensaCo.Entities
{
	public class Employee : Entity<int>
	{
		public DateTime DateHired { get; set; }
		public DateTimeOffset? DateLastWorked { get; set; }
		public DateTime? DateRelieved { get; set; }
		public EDepartment Department { get; set; }
		public string FirstName { get; set; }
		public string HomeAddress { get; set; }
		public string LastName { get; set; }
		public virtual Employee Manager { get; set; }
		public string MiddleName { get; set; }
		public decimal MonthlySalaryInEUR { get; set; }

		/// <summary>Personal identification number.</summary>
		public string PIN { get; set; }

		[ForeignKey(nameof(Position))]
		public string PositionId { get; set; }
		public virtual Role Position { get; set; }

		[ForeignKey(nameof(Profile))]
		public string ProfileId { get; set; }
		public virtual User Profile { get; set; }

		public byte Promotions { get; set; }
		public string WorkAddress { get; set; }

		public virtual MessageBox Messages { get; set; } = new MessageBox();
		public virtual ICollection<Employee> Workers { get; set; }

		public override string ToString()
		{
			return $"{FirstName} {MiddleName} {LastName}";
		}
	}
}
