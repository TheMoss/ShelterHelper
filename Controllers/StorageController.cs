using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShelterHelper.ViewModels;
using X.PagedList;

namespace ShelterHelper.Controllers
{
	public class StorageController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ILogger _logger;
		public StorageController(IHttpClientFactory httpClientFactory, ILogger<StorageController> logger)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
			
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
			return View("Views/Storage/Species/Index.cshtml", species);
		}

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

			return View("Views/Storage/Species/Create.cshtml", viewModel);
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

		//GET : StorageController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = new HttpResponseMessage();
			if (id == null) { return NotFound(); }

			response = await httpClient.GetAsync($"api/species/{id}");
			var speciesViewModel = new SpeciesViewModel();
			var availableSpeciesOptions = new SpeciesViewModel();
			//get all diets etc from the resources but use current species data to preselect initial diet value
			if (response.IsSuccessStatusCode)
			{
				speciesViewModel = await response.Content.ReadAsAsync<SpeciesViewModel>();
				
				HttpResponseMessage responseSpeciesOptions = await httpClient.GetAsync("api/resources");
				
				if (responseSpeciesOptions.IsSuccessStatusCode) {
					availableSpeciesOptions = await responseSpeciesOptions.Content.ReadAsAsync<SpeciesViewModel>();
				}
				else { return NotFound(); };

				speciesViewModel.AccessoriesList = availableSpeciesOptions.AccessoriesList;
				speciesViewModel.BeddingsList = availableSpeciesOptions.BeddingsList;
				speciesViewModel.DietsList = availableSpeciesOptions.DietsList;
				speciesViewModel.ToysList = availableSpeciesOptions.ToysList;

				speciesViewModel.SelectedAccessoryId = speciesViewModel.Accessory.AccessoryId;
				speciesViewModel.SelectedBeddingId = speciesViewModel.Bedding.BeddingId;
				speciesViewModel.SelectedDietId = speciesViewModel.Diet.DietId;
				speciesViewModel.SelectedToyId = speciesViewModel.Toy.ToyId;
			}

			if (speciesViewModel == null) { return NotFound(); }

			return View("Views/Storage/Species/Edit.cshtml", speciesViewModel);
		}
		//POST : StorageController/Edit/5
		[HttpPost, ValidateAntiForgeryToken]
		
		public async Task<IActionResult> ConfirmEditSpecies(int id, [Bind("SpeciesId", "SpeciesName", "SelectedDietId", "SelectedBeddingId", "SelectedToyId", "SelectedAccessoryId")] SpeciesViewModel speciesViewModel)
		{
			var speciesUpdate = new Models.Species(){
				SpeciesId = speciesViewModel.SpeciesId,
				SpeciesName = speciesViewModel.SpeciesName,
				DietId = speciesViewModel.SelectedDietId,
				BeddingId = speciesViewModel.SelectedBeddingId,
				ToyId = speciesViewModel.SelectedToyId,
				AccessoryId = speciesViewModel.SelectedAccessoryId
			};
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

			if (ModelState.IsValid)
			{
				try
				{
					HttpResponseMessage response = await httpClient.PostAsJsonAsync($"api/species/{speciesUpdate.SpeciesId}", speciesUpdate);
					response.EnsureSuccessStatusCode();
					TempData["Success"] = "Success, database updated.";
				}
				catch (HttpRequestException ex)
				{
					throw new HttpRequestException("Adding a new record to the database failed", ex);
				}

				return RedirectToAction("Species");
			}
			return View(speciesUpdate);
		}


        // GET: StorageController/Species/5
        public async Task<IActionResult> Delete(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            Models.Species species = null;
            if (id == null) { return NotFound(); }
            HttpResponseMessage response = await httpClient.GetAsync($"api/species/{id}");
            if (response.IsSuccessStatusCode)
            {
                species = await response.Content.ReadAsAsync<Models.Species>();
            }

            if (species == null) { return NotFound(); }
            return View("Views/Storage/Species/Delete.cshtml", species);
        }

        // POST: StorageController/Species/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int speciesId)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            HttpResponseMessage response = await httpClient.GetAsync($"api/species/{speciesId}");

            if (response.IsSuccessStatusCode)
            {
                await httpClient.DeleteAsync($"api/species/{speciesId}");
                TempData["Success"] = "Deleted succesfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete";
            }
            return RedirectToAction("Species");
        }


		#region Accessory
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

			return View("Views/Storage/EditResource/EditAccessory.cshtml", accessory);
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

		// GET: StorageController/Resources/Accessories/5
		public async Task<IActionResult> DeleteAccessory(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			Models.Accessory accessory = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/accessories/{id}");
			if (response.IsSuccessStatusCode)
			{
				accessory = await response.Content.ReadAsAsync<Models.Accessory>();
			}

			if (accessory == null) { return NotFound(); }
			return View("Views/Storage/DeleteResource/DeleteAccessory.cshtml", accessory);
		}

		// POST: StorageController/Resources/Accessories/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmDeleteAccessory(int accessoryId)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/accessories/{accessoryId}");

			if (response.IsSuccessStatusCode)
			{
				await httpClient.DeleteAsync($"api/resources/accessories/{accessoryId}");
				TempData["Success"] = "Deleted succesfully.";
			}
			else
			{
				TempData["Error"] = "Failed to delete";
			}
			return RedirectToAction("Index");
		}
		#endregion Accessory


		#region Bedding
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

			return View("Views/Storage/EditResource/EditBedding.cshtml", bedding);
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


		// GET: StorageController/Resources/Bedding/5
		public async Task<IActionResult> DeleteBedding(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			Models.Bedding bedding = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/beddings/{id}");
			if (response.IsSuccessStatusCode)
			{
				bedding = await response.Content.ReadAsAsync<Models.Bedding>();
			}

			if (bedding == null) { return NotFound(); }
			return View("Views/Storage/DeleteResource/DeleteBedding.cshtml", bedding);
		}

		// POST: StorageController/Resources/Bedding/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmDeleteBedding(int beddingId)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/beddings/{beddingId}");

			if (response.IsSuccessStatusCode)
			{
				await httpClient.DeleteAsync($"api/resources/beddings/{beddingId}");
				TempData["Success"] = "Deleted succesfully.";
			}
			else
			{
				TempData["Error"] = "Failed to delete";
			}
			return RedirectToAction("Index");
		}
		#endregion Bedding


		#region Diet
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

			return View("Views/Storage/EditResource/EditDiet.cshtml", diet);
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


		// GET: StorageController/Resources/Diets/5
		public async Task<IActionResult> DeleteDiet(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			Models.Diet diet= null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/diets/{id}");
			if (response.IsSuccessStatusCode)
			{
				diet = await response.Content.ReadAsAsync<Models.Diet>();
			}

			if (diet == null) { return NotFound(); }
			return View("Views/Storage/DeleteResource/DeleteDiet.cshtml", diet);
		}

		// POST: StorageController/Resources/Diets/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmDeleteDiet(int dietId)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/diets/{dietId}");

			if (response.IsSuccessStatusCode)
			{
				await httpClient.DeleteAsync($"api/resources/diets/{dietId}");
				TempData["Success"] = "Deleted succesfully.";
			}
			else
			{
				TempData["Error"] = "Failed to delete";
			}
			return RedirectToAction("Index");
		}
		#endregion Diet


		#region Toy		
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

			return View("Views/Storage/EditResource/EditToy.cshtml", toy);
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


		// GET: StorageController/Resources/Toys/5
		public async Task<IActionResult> DeleteToy(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			Models.Toy toy= null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/toys/{id}");
			if (response.IsSuccessStatusCode)
			{
				toy = await response.Content.ReadAsAsync<Models.Toy>();
			}

			if (toy == null) { return NotFound(); }
			return View("Views/Storage/DeleteResource/DeleteToy.cshtml", toy);
		}

		// POST: StorageController/Resources/Toys/5
		[HttpPost, ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmDeleteToy(int toyId)
		{
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			HttpResponseMessage response = await httpClient.GetAsync($"api/resources/toys/{toyId}");

			if (response.IsSuccessStatusCode)
			{
				await httpClient.DeleteAsync($"api/resources/toys/{toyId}");
				TempData["Success"] = "Deleted succesfully.";
			}
			else
			{
				TempData["Error"] = "Failed to delete";
			}
			return RedirectToAction("Index");
		}
		#endregion Toy
	}
}
