using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Employee
	{
		[Key]
		public int EmployeeId { get; set; }

		[Display(Name = "Personal ID")]
		public int? EmployeePersonalId{ get; set; }
		
        [StringLength(50)]
        [Display(Name = "Name")]
        public string? EmployeeName { get; set; }
        

        public Employee(string employeeName, int employeePersonalId)
        {
	        EmployeeName = employeeName;
	        EmployeePersonalId = employeePersonalId;
        }
    }
}