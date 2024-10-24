using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using ShelterHelperAPI.Models;
using System.Diagnostics;
using X.PagedList;
using X.PagedList.Extensions;

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

        // GET: HomeController
        public async Task<IActionResult> Index(int? page, string sortOrder, string searchString, string filterBy)
        {
            ViewBag.CurrentAnimalSortOrder = sortOrder;
            IEnumerable<Models.Animal> animals = null;
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var response = await httpClient.GetAsync("api/animals");

            if (response.IsSuccessStatusCode)
            {
                animals = await response.Content.ReadAsAsync<IEnumerable<Models.Animal>>();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (filterBy)
                {
                    case "name":
                        animals = animals.Where(name => name.Name.ToString().Contains(searchString));
                        break;
                    case "chipNumber":
                        animals = animals.Where(chipNumber => chipNumber.ChipNumber.ToString().Contains(searchString));
                        break;
                }
            }

            ViewBag.ChipNumberParam = sortOrder == "ChipNumber" ? "chip_number_desc" : "ChipNumber";
            ViewBag.AnimalsSortParam = sortOrder == "Species" ? "species_desc" : "Species";
            ViewBag.SexParam = sortOrder == "Sex" ? "sex_desc" : "Sex";
            ViewBag.AdmissionParam = sortOrder == "AdmissionDate" ? "admission_date_desc" : "AdmissionDate";
            ViewBag.AdoptionParam = sortOrder == "AdoptionDate" ? "adoption_date_desc" : "AdoptionDate";
            ViewBag.EmployeeParam = sortOrder == "Employee" ? "employee_desc" : "Employee";
            switch (sortOrder)
            {
                case "ChipNumber":
                    animals = animals.OrderBy(a => a.ChipNumber);
                    break;
                case "chip_number_desc":
                    animals = animals.OrderByDescending(a => a.ChipNumber);
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
                    animals = animals.OrderByDescending(a => a.Employee.EmployeePersonalId);
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
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: HomeController/Create
        public async Task<IActionResult> Create()
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var response = await httpClient.GetAsync("api/species");
            IEnumerable<Models.Species> species = null;
            var viewModel = new AnimalViewModel();
            if (response.IsSuccessStatusCode)
            {
                species = await response.Content.ReadAsAsync<IEnumerable<Models.Species>>();

                viewModel.SpeciesList = species.ToList();
            }

            return View(viewModel);
        }

        //POST : HomeController/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed(AnimalViewModel animalViewModel)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

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
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/animals", animal);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "New animal added to the database.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Failed to add a new record to the database.";
            }

            return View();
        }

        //GET: HomeController/Edit/1
        public async Task<IActionResult> Edit(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var animal = new Models.Animal();
            if (id == null)
            {
                return NotFound();
            }

            HttpResponseMessage response = await httpClient.GetAsync($"api/animals/{id}");
            if (response.IsSuccessStatusCode)
            {
                animal = await response.Content.ReadAsAsync<Models.Animal>();
            }

            return View(animal);
        }

        //POST: HomeController/Edit/1
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id,
            [Bind("Id, SpeciesId, Name, ChipNumber, Sex, Weight, AdmissionDay, AdoptionDay, Health, EmployeeId")]
            Models.Animal animal)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/animals/{animal.Id}", animal);
                response.EnsureSuccessStatusCode();
                TempData["Success"] = "Edited successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Failed to edit.";
            }

            return View(animal);
        }

        //GET : HomeController/Delete/1 
        public async Task<IActionResult> Delete(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var animal = new Models.Animal();
            if (id == null)
            {
                return NotFound();
            }

            var response = await httpClient.GetAsync($"api/animals/{id}");
            if (response.IsSuccessStatusCode)
            {
                animal = await response.Content.ReadAsAsync<Models.Animal>();
            }

            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        //POST : HomeController/Delete/1 
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var response = await httpClient.GetAsync($"api/animals/{id}");

            if (response.IsSuccessStatusCode)
            {
                await httpClient.DeleteAsync($"api/animals/{id}");
                TempData["Success"] = "Deleted successfully.";
            }

            return RedirectToAction("Index");
        }

        //GET : HomeController/Adopt/1
        public async Task<IActionResult> Adopt(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var animalViewModel = new AdoptionViewModel();
            if (id == null)
            {
                return NotFound();
            }

            var response = await httpClient.GetAsync($"api/animals/{id}");
            if (response.IsSuccessStatusCode)
            {
                Models.Animal animal = await response.Content.ReadAsAsync<Models.Animal>();
                animalViewModel.Animal = animal;
            }

            return View(animalViewModel);
        }

        //PATCH : HomeController/Adopt/1
        [HttpPatch, ActionName("Adopt")]
        public async Task<ActionResult> AdoptConfirmed(int animalId, AdoptionViewModel adoption)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            if (adoption.Animal.Id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var owner = new Models.Owner
                {
                    OwnerName = adoption.Owner.OwnerName,
                    Address = adoption.Owner.Address,
                    Email = adoption.Owner.Email
                };
                HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/owners", owner);
                response.EnsureSuccessStatusCode();

                var adoptionDate = adoption.Animal.AdoptionDay;

                var allOwners = await httpClient.GetAsync($"https://localhost:7147/api/owners");
                var ownersList = await allOwners.Content.ReadAsAsync<IEnumerable<Models.Owner>>();

                int? newOwnerId = ownersList.FirstOrDefault(owner =>
                    owner.OwnerName == adoption.Owner.OwnerName && owner.Address == adoption.Owner.Address).OwnerId;

                string patchDocument = "patch";
                HttpResponseMessage patchAdoptionDateAndOwner =
                    await httpClient.PatchAsJsonAsync($"api/animal/{animalId}", patchDocument);

                return RedirectToAction("Index");
            }

            return View(adoption);
        }
    }
}