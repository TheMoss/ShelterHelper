using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Animal
	{
		public int Id { get; set; }
		public string Species { get; set; }
		public string Name { get; set; }
		public string Sex { get; set; }
		public int Weight { get; set; }
		[Display(Name = "Admission day")]
		[DataType(DataType.Date)]
		public DateTime AdmissionDay { get; set; }
		[Display(Name = "Adoption day")]
		[DataType(DataType.Date)]
		public DateTime AdoptionDay { get; set; }
		public string Health { get; set; }
		[Display(Name = "Employee ID")]
		public int EmployeeId { get; set; }

	}
}
