using ShelterHelper.Models;

namespace ShelterHelper.ViewModels
{
	public class AnimalViewModel
	{
		public Animal? Animal { get; set; }        
        public virtual Species? Species { get; set; }
		public List<Species>? SpeciesList { get; set; }
		public Employee? Employee { get; set; }
	}
}
