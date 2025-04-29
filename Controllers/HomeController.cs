using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using X.PagedList.Extensions;

namespace ShelterHelper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AnimalsController _animalsController;
        private readonly SpeciesController _speciesController;
        private readonly OwnersController _ownersController;


        public HomeController(ILogger<HomeController> logger, AnimalsController animalsController,
            SpeciesController speciesController, OwnersController ownersController)
        {
            _logger = logger;
            _animalsController = animalsController;
            _speciesController = speciesController;
            _ownersController = ownersController;
        }

        // GET: HomeController
        public async Task<IActionResult> Index(int? page, string? searchString, string filterBy, string? sortOrder)
        {
            var allAnimals = await _animalsController.GetAnimalsDb();

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (filterBy)
                {
                    case "name":
                        allAnimals = allAnimals.Where(name => name.Name.ToString().Contains(searchString));
                        break;
                    case "chipNumber":
                        allAnimals = allAnimals.Where(chipNumber =>
                            chipNumber.ChipNumber.ToString().Contains(searchString));
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
                    allAnimals = allAnimals.OrderBy(a => a.ChipNumber);
                    break;
                case "chip_number_desc":
                    allAnimals = allAnimals.OrderByDescending(a => a.ChipNumber);
                    break;
                case "Species":
                    allAnimals = allAnimals.OrderBy(a => a.Species.SpeciesName);
                    break;
                case "species_desc":
                    allAnimals = allAnimals.OrderByDescending(a => a.Species.SpeciesName);
                    break;
                case "Sex":
                    allAnimals = allAnimals.OrderBy(a => a.Sex);
                    break;
                case "sex_desc":
                    allAnimals = allAnimals.OrderByDescending(a => a.Sex);
                    break;
                case "AdmissionDate":
                    allAnimals = allAnimals.OrderBy(a => a.AdmissionDay);
                    break;
                case "admission_date_desc":
                    allAnimals = allAnimals.OrderByDescending(a => a.AdmissionDay);
                    break;
                case "AdoptionDate":
                    allAnimals = allAnimals.OrderBy(a => a.AdoptionDay);
                    break;
                case "adoption_date_desc":
                    allAnimals = allAnimals.OrderByDescending(a => a.AdoptionDay);
                    break;
                case "Employee":
                    allAnimals = allAnimals.OrderBy(a => a.Employee.EmployeePersonalId);
                    break;
                case "employee_desc":
                    allAnimals = allAnimals.OrderByDescending(a => a.Employee.EmployeePersonalId);
                    break;
                default:
                    allAnimals = allAnimals.OrderBy(a => a.Name);
                    break;
            }

            var pageNumber = page ?? 1;
            var pagedList = allAnimals.ToPagedList(pageNumber, 10);
            ViewBag.AnimalsPagedList = pagedList;

            return View(allAnimals);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: HomeController/Create
        public async Task<IActionResult> Create()
        {
            var species = await _speciesController.GetSpeciesDb();
            var viewModel = new AnimalViewModel();
            viewModel.SpeciesList = species.ToList();

            return View(viewModel);
        }

        //POST : HomeController/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed(AnimalViewModel animalViewModel)
        {
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
                _animalsController.PostAnimal(animal);
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
            if (id == null)
            {
                return BadRequest();
            }

            var animal = await _animalsController.GetAnimal(id);
            if (animal.Value == null)
            {
                return NotFound();
            }

            return View(animal.Value);
        }

        //POST: HomeController/Edit/1
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id, SpeciesId, Name, ChipNumber, Sex, Weight, AdmissionDay, AdoptionDay, Health, EmployeeId")]
            Models.Animal animal)
        {
            if (ModelState.IsValid)
            {
                await _animalsController.PostAnimal(animal);
                TempData["Success"] = "Edited successfully.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to edit.";

            return View(animal);
        }

        //GET : HomeController/Delete/1 
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var animal = await _animalsController.GetAnimal(id);
            if (animal.Value == null)
            {
                return NotFound();
            }

            return View(animal.Value);
        }

        //POST : HomeController/Delete/1 
        [Authorize(Roles = "Manager")]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _animalsController.DeleteAnimal(id);
            return RedirectToAction("Index");
        }

        //GET : HomeController/Adopt/1
        public async Task<IActionResult> Adopt(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var adoptionViewModel = new AdoptionViewModel();
            var animal = await _animalsController.GetAnimal(id);
            if (animal.Value == null)
            {
                return NotFound();
            }

            adoptionViewModel.Animal = animal.Value;

            return View(adoptionViewModel);
        }

        //PATCH : HomeController/Adopt/1
        [HttpPost, ActionName("Adopt")]
        public async Task<ActionResult> AdoptConfirmed(int id, [FromForm] AdoptionViewModel adoption)
        {
            if (ModelState.IsValid)
            {
                var owner = new Models.Owner
                {
                    OwnerName = adoption.Owner.OwnerName,
                    Address = adoption.Owner.Address,
                    Email = adoption.Owner.Email
                };

                await _ownersController.Post(owner);
                var adoptionDate = adoption.Animal.AdoptionDay;

                var allOwners = await _ownersController.GetAllOwners();
                int? newOwnerId = allOwners.Value.First(owner => owner.OwnerName == adoption.Owner.OwnerName).OwnerId;
                if (newOwnerId == null)
                {
                    return NotFound();
                }

                var jsonPatch = new JsonPatchDocument<Animal>().Replace(a => a.AdoptionDay, adoptionDate)
                    .Replace(o => o.OwnerId, newOwnerId);
                await _animalsController.Patch(id, jsonPatch);
                TempData["Success"] = "Adoption recorded successfully.";
                return RedirectToAction("Index");
            }

            return View(adoption);
        }

        public IActionResult Welcome()
        {
            return View();
        }
    }
}