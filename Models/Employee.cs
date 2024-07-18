using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Employee
	{
		[Key]
		public int EmployeeId { get; set; }

		[Display(Name = "Employee's ID")]
		public int? EmployeePersonalId{ get; set; }
        [StringLength(50)]
        public string? EmployeeName { get; set; }

	}
}
