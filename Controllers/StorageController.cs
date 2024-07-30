using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using ShelterHelperAPI.Models;
using System.Net.Http;
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
		public async Task<IActionResult> Index()
		{

			SpeciesViewModel resources = null;
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync("/api/resources");
			if (response.IsSuccessStatusCode)
			{
				resources = await response.Content.ReadAsAsync<SpeciesViewModel>();
			}

			return View(resources);
		}

		public async Task<IActionResult> Species(int? page, string sortOrder)
		{
			ViewBag.CurrentStorageSortOrder = sortOrder;
			IEnumerable<ShelterHelper.Models.Species> species = null;
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync("/api/species");
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
		public IActionResult Details(int id)
		{

			return View();
		}

		public async Task<IActionResult> AddResources()
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync("api/resources");
			var viewModel = new SpeciesViewModel();
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
			}

			return View(viewModel);
		}

		#region Edit
		public async Task<IActionResult> EditAccessory(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = new HttpResponseMessage();
			if (id == null) { return NotFound(); }

			response = await httpClient.GetAsync($"api/resources/accessories/{id}");
			Models.Accessory accessory = new Models.Accessory();
			if (response.IsSuccessStatusCode)
			{
				accessory = await response.Content.ReadAsAsync<Models.Accessory>();
			}

			if (accessory == null) { return NotFound(); }

			return View("Views/Storage/Edit/EditAccessory.cshtml", accessory);
		}

		// POST: StorageController/Resources/Accessories/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmEditAccessory([Bind("AccessoryId","AccessoryName", "Quantity")] Models.Accessory accessory ) 
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			
			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/resources/accessories/{accessory.AccessoryId}", accessory);
					response.EnsureSuccessStatusCode();
					TempData["Success"] = "Success, database updated.";
				}
				catch (HttpRequestException ex)
				{
					throw new HttpRequestException("Adding a new record to the database failed", ex);
				}

				return RedirectToAction("Index");
			}
			return View(accessory);
		}
	
		public async Task<IActionResult> EditBedding(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = new HttpResponseMessage();
			if (id == null) { return NotFound(); }
			response = await httpClient.GetAsync($"api/resources/beddings/{id}");
			Models.Bedding bedding = new Models.Bedding();
			if (response.IsSuccessStatusCode)
			{
				bedding = await response.Content.ReadAsAsync<Models.Bedding>();
			}

			if (bedding == null) { return NotFound(); }

			return View("Views/Storage/Edit/EditBedding.cshtml", bedding);
		}

		// POST: StorageController/Resources/Bedding/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmEditBedding([Bind("BeddingId", "BeddingName", "Quantity_kg")] Models.Bedding bedding)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/resources/beddings/{bedding.BeddingId}", bedding);
					response.EnsureSuccessStatusCode();
					TempData["Success"] = "Success, database updated.";
				}
				catch (HttpRequestException ex)
				{
					throw new HttpRequestException("Adding a new record to the database failed", ex);
				}

				return RedirectToAction("Index");
			}
			return View(bedding);
		}

		public async Task<IActionResult> EditDiet(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = new HttpResponseMessage();
			if (id == null) { return NotFound(); }
			response = await httpClient.GetAsync($"api/resources/diets/{id}");
			Models.Diet diet = new Models.Diet();
			if (response.IsSuccessStatusCode)
			{
				diet = await response.Content.ReadAsAsync<Models.Diet>();
			}

			if (diet == null) { return NotFound(); }

			return View("Views/Storage/Edit/EditDiet.cshtml", diet);
		}

		// POST: StorageController/Resources/Diets/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmEditDiet([Bind("DietId", "DietName", "Quantity_kg")] Models.Diet diet)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/resources/diets/{diet.DietId}", diet);
					response.EnsureSuccessStatusCode();
					TempData["Success"] = "Success, database updated.";
				}
				catch (HttpRequestException ex)
				{
					throw new HttpRequestException("Adding a new record to the database failed", ex);
				}

				return RedirectToAction("Index");
			}
			return View(diet);
		}
		public async Task<IActionResult> EditToy(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = new HttpResponseMessage();
			if (id == null) { return NotFound(); }
			response = await httpClient.GetAsync($"api/resources/toys/{id}");
			Models.Toy toy = new Models.Toy();
			if (response.IsSuccessStatusCode)
			{
				toy = await response.Content.ReadAsAsync<Models.Toy>();
			}

			if (toy == null) { return NotFound(); }

			return View("Views/Storage/Edit/EditToy.cshtml", toy);
		}

		// POST: StorageController/Resources/Toys/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmEditToy([Bind("ToyId", "ToyName", "Quantity")] Models.Toy toy)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/resources/toys/{toy.ToyId}", toy);
					response.EnsureSuccessStatusCode();
					TempData["Success"] = "Success, database updated.";
				}
				catch (HttpRequestException ex)
				{
					throw new HttpRequestException("Adding a new record to the database failed", ex);
				}

				return RedirectToAction("Index");
			}
			return View(toy);
		}
		#endregion Edit
		#region Create
		// GET: StorageController/Create
		public async Task<IActionResult> Create()
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync("api/resources");
			var viewModel = new SpeciesViewModel();
			if (response.IsSuccessStatusCode)
			{
				string content = await response.Content.ReadAsStringAsync();
				viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
			}

			return View(viewModel);
		}
		// POST: StorageController/Create
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SpeciesViewModel viewModel)
		{ //id as value
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
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
					HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/species", species);
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

		// POST: StorageController/Diet
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNewDiet(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			var diet = new Models.Diet()
			{
				DietName = viewModel.Diet.DietName,
				Quantity_kg = viewModel.Diet.Quantity_kg
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/resources/diets", diet);
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
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNewBedding(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			var bedding = new Models.Bedding()
			{
				BeddingName = viewModel.Bedding.BeddingName,
				Quantity_kg = viewModel.Bedding.Quantity_kg
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/resources/beddings", bedding);
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
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateNewToy(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			var toy = new Models.Toy()
			{
				ToyName = viewModel.Toy.ToyName,
				Quantity = viewModel.Toy.Quantity
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/resources/toys", toy);
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
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNewAccessory(SpeciesViewModel viewModel)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			var accessory = new Models.Accessory()
			{
				AccessoryName = viewModel.Accessory.AccessoryName,
				Quantity = viewModel.Accessory.Quantity
			};

			if (ModelState.IsValid)
			{
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/resources/accessories", accessory);
				response.EnsureSuccessStatusCode();
				TempData["Success"] = "Success, new accessory type added.";
			}
			else
			{
				TempData["Error"] = "Error, something went wrong.";
			}
			return RedirectToAction("Create");
		}

		
		#endregion Create
		// GET: StorageController/Edit/5
		public IActionResult Edit(int id)
		{
			return View();
		}

		// POST: StorageController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(int id, IFormCollection collection)
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
		public IActionResult Delete(int id)
		{
			return View();
		}

		// POST: StorageController/Delete/5
		[HttpPost, ValidateAntiForgeryToken]
		public IActionResult Delete(int id, IFormCollection collection)
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
