using ShelterHelper.Models;
using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.ViewModels
{
	public class AnimalViewModel
	{
		public Animal? Animal { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Species")]
        public int SpeciesId { get; set; }
        public virtual Species? Species { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        public string Sex { get; set; }

        [Required]
        public int Weight { get; set; }

        [Required]
        [Display(Name = "Admission day")]
        [DataType(DataType.Date)]
        public DateOnly AdmissionDay { get; set; }

        [Display(Name = "Adoption day")]
        [DataType(DataType.Date)]
        public DateOnly? AdoptionDay { get; set; }

        [Required]
        public string Health { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public int EmployeeId { get; set; }
        public List<Species>? SpeciesList { get; set; }
	}
}
