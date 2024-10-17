
using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Assignment
	{
		public int? AssignmentId { get; set; }
		[Required]
		[StringLength(100)]
		public string Title { get; set; }
		[StringLength(300)]
		public string? Description { get; set; }
		[Range(0,3)]
		public int Priority { get; set; }
		[Display(Name = "Creator's ID")]
		public int CreatorId {  get; set; }
		[Display(Name = "Creation date")]
		public DateOnly? CreationDate { get; set; }
		public bool? IsCompleted { get; set; } = false;
		public bool? IsInProgress { get; set; } = false;

	}
}
