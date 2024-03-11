using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

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

		public async Task<ActionResult> Index()
		{

			IEnumerable<Animal> animals = null;
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Animals");
			if (response.IsSuccessStatusCode)
			{
				animals = await response.Content.ReadAsAsync<IEnumerable<Animal>>();
			}
			return View(animals);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public ActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public ActionResult Create()
		{			
			return View();
		}

		[HttpPost, ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateConfirmed([Bind("Id, Species, Name, Sex, Weight, AdmissionDay, AdoptionDay, Health, EmployeeId")] Animal animal)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");

			animal.AdoptionDay = new DateOnly(1900, 1, 1);
			if (ModelState.IsValid)
			{					
				HttpResponseMessage response = await httpClient.PostAsJsonAsync($"https://localhost:7147/api/Animals", animal);
				response.EnsureSuccessStatusCode();
				return RedirectToAction("Index");
			}

			return View(animal);
		}

		public async Task<ActionResult> Edit(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			Animal animal = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{
				animal = await response.Content.ReadAsAsync<Animal>();
			}

			if (animal == null) { return NotFound(); }

			return View(animal);
		}
		
		public async Task<ActionResult> Delete(int? id)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			Animal animal = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{  
				animal = await response.Content.ReadAsAsync<Animal> ();
			}

			if (animal == null) { return NotFound (); }
			return View(animal);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<ActionResult> DeleteConfirmed(int? id)
		{
			if (id == null) { return NotFound(); }
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");

			if (response.IsSuccessStatusCode)
			{
				await httpClient.DeleteAsync($"https://localhost:7147/api/Animals/{id}");
			}

			return RedirectToAction("Index");
		}
	}
}
