using Microsoft.AspNetCore.Mvc;
using ShelterHelper.ViewModels;
using X.PagedList;
using Newtonsoft.Json;

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
			var pagedList = species.ToPagedList(pageNumber, 2);
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
				//check if entry exists or create a new one
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Species", species);
				response.EnsureSuccessStatusCode();
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
