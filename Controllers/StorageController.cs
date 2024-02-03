using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;

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
		public async Task <IActionResult> Index()
		{
            IEnumerable<Species> species = null;
            var httpClient = _httpClientFactory.CreateClient("Client");
            HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Species");
            if (response.IsSuccessStatusCode)
            {
                species = await response.Content.ReadAsAsync<IEnumerable<Species>>();
            }
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
