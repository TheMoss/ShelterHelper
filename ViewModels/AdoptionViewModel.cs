using ShelterHelper.Models;

namespace ShelterHelper.ViewModels
{
    public class AdoptionViewModel
    {
        public Owner Owner { get; set; }
        public Animal Animal { get; set; }
        public Employee Employee { get; set; }
        public List<Animal>? AnimalList { get; set; }
    }
}