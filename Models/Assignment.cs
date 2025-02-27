using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShelterHelper.Models
{
    public class Assignment
    {
        public int? AssignmentId { get; set; }

        [Required] 
        [StringLength(100)] 
        public string Title { get; set; } = null!;
        
        [StringLength(300)]
        public string? Description { get; set; }
        
        [Required]
        [Range(0, 3)]
        public int Priority { get; set; }

        [Required]
        [Display(Name = "Creator's ID")]
        public int CreatorId { get; set; }

        [Display(Name = "Creation date")]
        public DateOnly? CreationDate { get; set; }
        
        [Display(Name = "Is completed?")]
        public bool? IsCompleted { get; set; } = false;
        
        [Display(Name = "Is in progress?")]
        public bool? IsInProgress { get; set; } = false;

        public List<SelectListItem> PrioritiesSelectList = new List<SelectListItem>
        {
            new SelectListItem { Text = "Low", Value = "0" },
            new SelectListItem { Text = "Medium", Value = "1" },
            new SelectListItem { Text = "High", Value = "2" },
            new SelectListItem { Text = "Very high", Value = "3" }
        };
    }
    
}