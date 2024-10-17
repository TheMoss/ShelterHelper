using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;

namespace ShelterHelper.Controllers
{
	public class AssignmentsController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IHttpClientFactory _httpClientFactory;


		public AssignmentsController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;

		}
		// GET: AssignmentsController
		public async Task<IActionResult> Index()
		{
			IEnumerable<Assignment> assignments = null;
			var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
			var response = await httpClient.GetAsync("api/assignments");
			
			if (response.IsSuccessStatusCode)
			{
				assignments = await response.Content.ReadAsAsync<IEnumerable<Assignment>>();
			}
			return View(assignments);
		}
		// GET: AssignmentsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: AssignmentsController/Create
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

		// GET: AssignmentsController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: AssignmentsController/Edit/5
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

		// GET: AssignmentsController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: AssignmentsController/Delete/5
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
