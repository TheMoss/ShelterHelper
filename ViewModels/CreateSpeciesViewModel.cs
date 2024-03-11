using ShelterHelper.Models;
namespace ShelterHelper.ViewModels
{
    public class CreateSpeciesViewModel
    {
        public string? SpeciesName { get; set; }
        public int SelectedDietId { get; set; }
        public Diet? Diet { get; set; }
        public List<Diet>? DietsList { get; set; }
        public int SelectedBeddingId { get; set; }
        public Bedding? Bedding { get; set; }
        public List<Bedding>? BeddingsList { get; set; }
        public int SelectedToyId { get; set; }
        public Toy? Toy { get; set; }
        public List<Toy>? ToysList { get; set; }
        public int SelectedAccessoryId { get; set; }
        public Accessory? Accessory { get; set; }
        public List<Accessory>? AccessoriesList { get; set; }
    }
}
