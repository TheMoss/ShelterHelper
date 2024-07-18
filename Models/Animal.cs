using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Animal
	{
		[Key]
		public int? Id { get; set; }

		[Display(Name = "Species")]
		public int? SpeciesId { get; set; }

		public virtual Species? Species { get; set; }

		[Display(Name = "Chip number"), Range(10000000, 99999999), Required]
		public int ChipNumber { get; set; }

		[StringLength(30, MinimumLength = 3), Required]
		public string Name { get; set; }

		public string? Sex { get; set; }

		[Required]
        [Range(1, 5000, ErrorMessage = "Only positive number allowed")]
        public int Weight { get; set; }

		[DataType(DataType.Date), Display(Name = "Admission day"), Required]
		public DateOnly AdmissionDay { get; set; }

		[DataType(DataType.Date), Display(Name = "Adoption day")]
		public DateOnly? AdoptionDay { get; set; }

		[StringLength(300)]
		public string? Health { get; set; }

		public virtual Employee? Employee { get; set; }
		public int EmployeeId { get; set; }
		public int EmployeePersonalId { get; set; }

		public virtual Owner? Owner { get; set; }
		public int? OwnerId { get; set; }

	}

}
