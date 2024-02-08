using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
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
		public async Task <IActionResult> Index(int? page, string sortOrder)
		{
            ViewBag.CurrentSortOrder = sortOrder;
            IEnumerable<Species> species = null;
            var httpClient = _httpClientFactory.CreateClient("Client");
            HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Species");
            if (response.IsSuccessStatusCode)
            {
                species = await response.Content.ReadAsAsync<IEnumerable<Species>>();
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
		public ActionResult Create()
		{
			return View();
		}

		// POST: StorageController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
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
