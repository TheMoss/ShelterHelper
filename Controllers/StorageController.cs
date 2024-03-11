using Microsoft.AspNetCore.Mvc;
using ShelterHelper.ViewModels;
using X.PagedList;
using Newtonsoft.Json;
using ShelterHelper.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
			ViewBag.CurrentSortOrder = sortOrder;
			IEnumerable<ShelterHelper.Models.Species> species = null;
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Species");
			if (response.IsSuccessStatusCode)
			{
				species = await response.Content.ReadAsAsync<IEnumerable<Models.Species>>();
			}

			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

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

		// GET: StorageController/Create
		public async Task<ActionResult> Create()
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/GetStorage");
			var viewModel = new CreateSpeciesViewModel();
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				viewModel = JsonConvert.DeserializeObject<CreateSpeciesViewModel>(content);
			}		
			
			return View(viewModel);
		}
		// POST: StorageController/Diet
		[HttpPost]
		public async Task<ActionResult> CreateNewDiet(CreateSpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var diet = new Diet()
			{
				DietName = viewModel.Diet.DietName,
				Quantity_kg = viewModel.Diet.Quantity_kg
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Storage/Diet", diet);
				response.EnsureSuccessStatusCode();
				TempData["Message"] = "Operation successful";
				return RedirectToAction("Index");
			}
			return RedirectToAction("Create");
		}
		// POST: StorageController/Bedding
		[HttpPost]
		public async Task<ActionResult> CreateNewBedding(CreateSpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var bedding = new Bedding()
			{
				BeddingName = viewModel.Bedding.BeddingName,
				Quantity_kg = viewModel.Bedding.Quantity_kg
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Storage/Bedding", bedding);
				response.EnsureSuccessStatusCode();
				TempData["Message"] = "Operation successful";
				return RedirectToAction("Index");
			}
			return RedirectToAction("Create");
		}
		// POST: StorageController/Toy
		[HttpPost]
		public async Task<ActionResult> CreateNewToy(CreateSpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var toy = new Toy()
			{
				ToyName = viewModel.Toy.ToyName,
				Quantity = viewModel.Toy.Quantity
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Storage/Toy", toy);
				response.EnsureSuccessStatusCode();
				TempData["Message"] = "Operation successful";
				return RedirectToAction("Index");
			}
			return RedirectToAction("Create");
		}
		// POST: StorageController/Accessory
		[HttpPost]
		public async Task<ActionResult> CreateNewAccessory(CreateSpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			var accessory = new Accessory()
			{
				AccessoryName = viewModel.Accessory.AccessoryName,
				Quantity = viewModel.Accessory.Quantity
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Storage/Bedding", accessory);
				response.EnsureSuccessStatusCode();
				TempData["Message"] = "Operation successful";
				return RedirectToAction("Index");
			}
			return RedirectToAction("Create");
		}

		// POST: StorageController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateSpeciesViewModel viewModel)
		{ //id as value
			var httpClient = _httpClientFactory.CreateClient("Client");
			var species = new Models.Species()
			{
				SpeciesName = viewModel.SpeciesName,
				DietId = viewModel.SelectedDietId,
				BeddingId = viewModel.SelectedBeddingId,
				ToyId = viewModel.SelectedToyId,
				AccessoryId = viewModel.SelectedAccessoryId,
			};

			
			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Species", species);
                    response.EnsureSuccessStatusCode();
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
