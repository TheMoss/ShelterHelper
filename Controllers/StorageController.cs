using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShelterHelper.ViewModels;
using X.PagedList;

namespace ShelterHelper.Controllers
{
	public class StorageController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		public StorageController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		// GET: StorageController
		public async Task<IActionResult> Index(int? page, string sortOrder)
		{
			ViewBag.CurrentStorageSortOrder = sortOrder;
			IEnumerable<ShelterHelper.Models.Species> species = null;
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/species");
			if (response.IsSuccessStatusCode)
			{
				species = await response.Content.ReadAsAsync<IEnumerable<Models.Species>>();
			}

			ViewBag.StorageSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

			switch (sortOrder)
			{
				case "name_desc":
					species = species.OrderByDescending(s => s.SpeciesName);
					break;
				default:
					species = species.OrderBy(s => s.SpeciesName);
					break;
			}
			var pageNumber = page ?? 1;
			var pagedList = species.ToPagedList(pageNumber, 10);
			ViewBag.PagedList = pagedList;
			return View(species);
		}



		// GET: StorageController/Details/5
		public ActionResult Details(int id)
		{

			return View();
		}

		public async Task<ActionResult> AddResources()
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/resources");
			var viewModel = new SpeciesViewModel();
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
			}

			return View(viewModel);
		}
		

		// GET: StorageController/Create
		public async Task<ActionResult> Create()
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/resources");
			var viewModel = new SpeciesViewModel();
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
			}
			
			return View(viewModel);
		}
		// POST: StorageController/Diet
		[HttpPost]
		public async Task<ActionResult> CreateNewDiet(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var diet = new Models.Diet()
			{
				DietName = viewModel.Diet.DietName,
				Quantity_kg = viewModel.Diet.Quantity_kg
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/resources/diets", diet);
				response.EnsureSuccessStatusCode();
                TempData["Success"] = "Success, new diet type added.";
            }
            else
            {
                TempData["Error"] = "Error, something went wrong.";
            }
            return RedirectToAction("Create");
		}
		// POST: StorageController/Bedding
		[HttpPost]
		public async Task<ActionResult> CreateNewBedding(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var bedding = new Models.Bedding()
			{
				BeddingName = viewModel.Bedding.BeddingName,
				Quantity_kg = viewModel.Bedding.Quantity_kg
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/resources/beddings", bedding);
				response.EnsureSuccessStatusCode();
                TempData["Success"] = "Success, new bedding type added.";
            }
            else
            {
                TempData["Error"] = "Error, something went wrong.";
            }
            return RedirectToAction("Create");
		}
		// POST: StorageController/Toy
		[HttpPost]
		public async Task<ActionResult> CreateNewToy(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var toy = new Models.Toy()
			{
				ToyName = viewModel.Toy.ToyName,
				Quantity = viewModel.Toy.Quantity
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/resources/toys", toy);
				response.EnsureSuccessStatusCode();
                TempData["Success"] = "Success, new toy type added.";
            }
            else
            {
                TempData["Error"] = "Error, something went wrong.";
            }
            return RedirectToAction("Create");
		}
		// POST: StorageController/Accessory
		[HttpPost]
		public async Task<ActionResult> CreateNewAccessory(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var accessory = new Models.Accessory()
			{
				AccessoryName = viewModel.Accessory.AccessoryName,
				Quantity = viewModel.Accessory.Quantity
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/resources/accessories", accessory);
				response.EnsureSuccessStatusCode();
                TempData["Success"] = "Success, new accessory type added.";
            }
            else
            {
                TempData["Error"] = "Error, something went wrong.";
            }
            return RedirectToAction("Create");
		}

		// POST: StorageController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(SpeciesViewModel viewModel)
		{ //id as value
			var httpClient = _httpClientFactory.CreateClient("Client");
			var species = new Models.Species()
			{
				SpeciesName = viewModel.Species.SpeciesName,
				DietId = viewModel.SelectedDietId,
				BeddingId = viewModel.SelectedBeddingId,
				ToyId = viewModel.SelectedToyId,
				AccessoryId = viewModel.SelectedAccessoryId,
			};

			
			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/species", species);
                    response.EnsureSuccessStatusCode();
                    TempData["Success"] = "Success, database updated.";
                }
				catch (HttpRequestException ex)
				{
					throw new HttpRequestException("Adding a new record to the database failed", ex);
				}
                
				return RedirectToAction("Index");
			}
			return View(species);
		}

		// GET: StorageController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: StorageController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: StorageController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: StorageController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}	
		
	}
}
