using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using ShelterHelperAPI.Models;
using System.Diagnostics;
using X.PagedList;

namespace ShelterHelper.Controllers
{
    public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;		
		private readonly IHttpClientFactory _httpClientFactory;


		public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;

		}

		public async Task<ActionResult> Index(int? page, string sortOrder)
		{
			ViewBag.CurrentAnimalSortOrder = sortOrder;
			IEnumerable<Models.Animal> animals = null;
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Animals");
			
			if (response.IsSuccessStatusCode)
			{
				animals = await response.Content.ReadAsAsync<IEnumerable<Models.Animal>>();
			}

			ViewBag.ChipNumberParam = sortOrder == "ChipNumber" ? "chip_number_desc" : "ChipNumber";
			ViewBag.AnimalsSortParam = sortOrder == "Species" ? "species_desc" : "Species";
			ViewBag.SexParam = sortOrder == "Sex" ? "sex_desc" : "Sex";
			ViewBag.AdmissionParam = sortOrder == "AdmissionDate" ? "admission_date_desc" : "AdmissionDate";
			ViewBag.AdoptionParam = sortOrder == "AdoptionDate" ? "adoption_date_desc" : "AdoptionDate";
			ViewBag.EmployeeParam = sortOrder == "Employee" ? "employee_desc" : "Employee";
			switch(sortOrder)
			{
				case "ChipNumber":
					animals = animals.OrderBy(a => a.ChipNumber);
					break;
				case "chip_number_desc":
					animals=animals.OrderByDescending(a => a.ChipNumber);
					break;
				case "Species":
					animals = animals.OrderBy(a => a.Species.SpeciesName);
					break;
				case "species_desc":
					animals = animals.OrderByDescending(a => a.Species.SpeciesName);
					break;
				case "Sex":
					animals = animals.OrderBy(a => a.Sex);
					break;
				case "sex_desc":
					animals = animals.OrderByDescending(a => a.Sex);
					break;
				case "AdmissionDate":
					animals = animals.OrderBy(a => a.AdmissionDay);
					break;
				case "admission_date_desc":
					animals = animals.OrderByDescending(a => a.AdmissionDay);
					break;
				case "AdoptionDate":
					animals = animals.OrderBy(a => a.AdoptionDay);
					break;
				case "adoption_date_desc":
					animals = animals.OrderByDescending(a => a.AdoptionDay);
					break;
				case "Employee":
					animals = animals.OrderBy(a => a.Employee.EmployeePersonalId);
					break;
				case "employee_desc":
					animals = animals.OrderByDescending(a=>a.Employee.EmployeePersonalId);
					break;
				default:
					animals = animals.OrderBy(a => a.Name);
					break;
			}
			var pageNumber = page ?? 1;
			var pagedList = animals.ToPagedList(pageNumber, 10);
			ViewBag.AnimalsPagedList = pagedList;
			return View(animals);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public ActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task<ActionResult> Create()
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Species");
			IEnumerable<Models.Species> species = null;
			var viewModel = new AnimalViewModel();
			if (response.IsSuccessStatusCode)
			{				
				species = await response.Content.ReadAsAsync<IEnumerable<Models.Species>>();

				viewModel.SpeciesList = species.ToList();
			}

			return View(viewModel);
		}

		[HttpPost, ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateConfirmed(AnimalViewModel animalViewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");

            var animal = new Models.Animal()
            {
                SpeciesId = animalViewModel.Animal.SpeciesId,
				ChipNumber = animalViewModel.Animal.ChipNumber,
                Name = animalViewModel.Animal.Name,
                Sex = animalViewModel.Animal.Sex,
                Weight = animalViewModel.Animal.Weight,
                AdmissionDay = animalViewModel.Animal.AdmissionDay,
				Health = animalViewModel.Animal.Health,
				EmployeeId = animalViewModel.Animal.EmployeeId
            };

            animal.AdoptionDay = new DateOnly(1900, 1, 1);
			if (ModelState.IsValid)
			{					
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Animals", animal);
				response.EnsureSuccessStatusCode();
				return RedirectToAction("Index");
			}

			return View(animal);
		}

		public async Task<ActionResult> Edit(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
            Models.Animal animal = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{
				animal = await response.Content.ReadAsAsync<Models.Animal>();
			}

			if (animal == null) { return NotFound(); }

			return View(animal);
		}
		
		public async Task<ActionResult> Delete(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			Models.Animal animal = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{  
				animal = await response.Content.ReadAsAsync<Models.Animal> ();
			}

			if (animal == null) { return NotFound (); }
			return View(animal);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<ActionResult> DeleteConfirmed(int? id)
		{
			if (id == null) { return NotFound(); }
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");

			if (response.IsSuccessStatusCode)
			{
				await httpClient.DeleteAsync($"https://localhost:7147/api/Animals/{id}");
			}

			return RedirectToAction("Index");
		}
		public async Task<ActionResult> Adopt(int? id)
	{
			var httpClient = _httpClientFactory.CreateClient("Client");
			AdoptionViewModel animalViewModel = new AdoptionViewModel();
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{
				Models.Animal animal = await response.Content.ReadAsAsync<Models.Animal>();
				animalViewModel.Animal = animal;
				//get toy and accessory
			}

			if (animalViewModel == null) { return NotFound(); }


			return View(animalViewModel);
	}
	}

	
}
