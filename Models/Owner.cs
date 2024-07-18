using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Owner
	{
		public int? OwnerId { get; set; }
		[Display(Name = "Owner's name"), StringLength(50), Required]		
		public string OwnerName { get; set; }
		[Required, StringLength(200)]
		public string Address { get; set; }
		[EmailAddress(ErrorMessage ="Invalid email address")]
		public string? Email { get; set; }
	}
}
