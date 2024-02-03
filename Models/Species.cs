using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models
{
	public class Species
	{
		[Key]
		public int SpeciesId { get; set; }
        [Display(Name = "Species")]
        public string SpeciesName { get; set; }
		public int DietId { get; set; }
		public virtual Diet Diet { get; set; }
		public int BeddingId { get; set; }
		public virtual Bedding Bedding { get; set; }
		public int ToyId { get; set; }
		public virtual Toy Toy { get; set; }
		public int AccessoryId { get; set; }
		public virtual Accessory Accessory { get; set; }
	}



	public class Diet
	{
		[Key]
		public int DietId { get; set; }
        [Display(Name = "Diet")]
        public string DietName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity_kg { get; set; }
	}

	public class Bedding
	{
		[Key]
		public int BeddingId { get; set; }
        [Display(Name = "Bedding")]
        public string BeddingName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity_kg { get; set; }
	}

	public class Toy
	{
		[Key]
		public int ToyId { get; set; }
        [Display(Name = "Toy")]
        public string ToyName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
	}

	public class Accessory
	{
		[Key]
		public int AccessoryId { get; set; }
        [Display(Name = "Accessory")]
        public string AccessoryName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
	}
}
