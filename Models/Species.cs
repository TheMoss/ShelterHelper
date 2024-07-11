using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShelterHelper.Models
{
	public class Species
	{
		[Key]		
		public int? SpeciesId { get; set; }

        [Display(Name = "Species"), StringLength(20, MinimumLength = 3)]		
		public string SpeciesName { get; set; }

		public int DietId { get; set; }		
		public virtual Diet? Diet { get; set; }

		public int BeddingId { get; set; }		
		public virtual Bedding? Bedding { get; set; }

		public int ToyId { get; set; }		
		public virtual Toy? Toy { get; set; }

		public int AccessoryId { get; set; }		
		public virtual Accessory? Accessory { get; set; }
	}



	public class Diet
	{
		[Key]
		public int? DietId { get; set; }
        [Display(Name = "Diet"), StringLength(20, MinimumLength = 3)]
        public string DietName { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity_kg { get; set; }
	}

	public class Bedding
	{
		[Key]
		public int? BeddingId { get; set; }

        [Display(Name = "Bedding"), StringLength(20, MinimumLength = 3)]
		public string BeddingName { get; set; }

        [Display(Name = "Quantity"), Range(1, Int32.MaxValue)]		
        public int Quantity_kg { get; set; }
	}

	public class Toy
	{
		[Key]
		public int? ToyId { get; set; }

        [Display(Name = "Toy"), StringLength(20, MinimumLength = 3)]
		public string ToyName { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
	}

	public class Accessory
	{
		[Key]
		public int? AccessoryId { get; set; }

        [Display(Name = "Accessory"), StringLength(20, MinimumLength = 3)]
		public string AccessoryName { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
	}
}
