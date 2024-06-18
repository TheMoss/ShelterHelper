using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Employee
	{
		[Key]
		public int EmployeeId { get; set; }
		[Display(Name = "Employee ID")]
		[Length(minimumLength: 6, maximumLength: 6)]
		public int EmployeePersonalId{ get; set; }
		public string EmployeeName { get; set; }

	}
}
