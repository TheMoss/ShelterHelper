using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Owner
	{
		public int OwnerId { get; set; }
		[Display(Name = "Owner's name"), Required]		
		public string OwnerName { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		public string Email { get; set; }
	}
}
