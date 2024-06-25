using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
    public class Animal
    {
        [Key]
        public int? Id { get; set; }
		[Required(ErrorMessage = "This field is required")]
		[Display(Name = "Species")]
		public int SpeciesId { get; set; }
        public virtual Species? Species { get; set; }
        [Display(Name = "Chip number")]
        //[Length(minimumLength: 8, maximumLength: 8)]
        [Range(10000000, 99999999)]
        public int ChipNumber { get; set; }

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

        public virtual Employee? Employee { get; set; }
        public int EmployeeId { get; set; }
        public int EmployeePersonalId {  get; set; }
        
    }

}
