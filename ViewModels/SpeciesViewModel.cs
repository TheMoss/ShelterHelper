using ShelterHelper.Models;
using System.ComponentModel.DataAnnotations;
namespace ShelterHelper.ViewModels
{
    public class SpeciesViewModel
    {
		public int? SpeciesId { get; set; }
        public string? SpeciesName { get;set; }
		public Species? Species { get; set; }

		[Display(Name = "Choose a diet")]
		public int? SelectedDietId { get; set; }
        public Diet? Diet { get; set; }
        public List<Diet>? DietsList { get; set; }
		
        [Display(Name = "Choose a bedding")]
        public int? SelectedBeddingId { get; set; }
        public Bedding? Bedding { get; set; }
        public List<Bedding>? BeddingsList { get; set; }

		[Display(Name = "Choose a toy")]
		public int? SelectedToyId { get; set; }
        public Toy? Toy { get; set; }
        public List<Toy>? ToysList { get; set; }

		[Display(Name = "Choose an accessory")]
		public int? SelectedAccessoryId { get; set; }
        public Accessory? Accessory { get; set; }
        public List<Accessory>? AccessoriesList { get; set; }
    }
}
