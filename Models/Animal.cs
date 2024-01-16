using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
    public class Animal
    {
        public int Id { get; set; }

        [Required]
        public string Species { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        public string Sex { get; set; }

        [Required]
        public int Weight { get; set; }

        [Required]
        [Display(Name = "Admission day")]
        [DataType(DataType.Date)]
        public DateTime AdmissionDay { get; set; }

        [Display(Name = "Adoption day")]
        [DataType(DataType.Date)]
        public DateTime AdoptionDay { get; set; }

        [Required]
        public string Health { get; set; }

        [Required]
        [Display(Name = "Employee ID")]        
        public int EmployeeId { get; set; }
    }

}
