using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using ShelterHelper.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ShelterHelper.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;		
		private readonly HttpClient _httpClient = null!;


		public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
		{
			_logger = logger;
			_httpClient = httpClient;

			_httpClient.BaseAddress = new Uri("https://localhost:7147/");
			_httpClient.DefaultRequestHeaders.Accept.Clear();
			_httpClient.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<IActionResult> Index()
		{

			IEnumerable<Animal> animals = null;

			HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7147/api/Animals");
			if (response.IsSuccessStatusCode)
			{
				animals = await response.Content.ReadAsAsync<IEnumerable<Animal>>();
			}
			return View(animals);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult Create()
		{			
			return View();
		}

		[HttpPost, ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateConfirmed([Bind("Id, Species, Name, Sex, Weight, AdmissionDay, AdoptionDay, Health, EmployeeId")] Animal animal)
		{
			animal.AdoptionDay = new DateOnly(1900, 1, 1);
			if (ModelState.IsValid)
			{					
				HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"https://localhost:7147/api/Animals", animal);
				response.EnsureSuccessStatusCode();
				return RedirectToAction("Index");
			}

			return View(animal);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			Animal animal = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{
				animal = await response.Content.ReadAsAsync<Animal>();
			}

			if (animal == null) { return NotFound(); }

			return View(animal);
		}
		
		public async Task<IActionResult> Delete(int? id)
		{
			Animal animal = null;
			if (id == null) { return NotFound(); }
			HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");
			if (response.IsSuccessStatusCode)
			{  
				animal = await response.Content.ReadAsAsync<Animal> ();
			}

			if (animal == null) { return NotFound (); }
			return View(animal);
		}

		[HttpPost, ActionName("Delete")]
		public async Task<IActionResult> DeleteConfirmed(int? id)
		{
			if (id == null) { return NotFound(); }

			HttpResponseMessage response = await _httpClient.GetAsync($"https://localhost:7147/api/Animals/{id}");

			if (response.IsSuccessStatusCode)
			{
				await _httpClient.DeleteAsync($"https://localhost:7147/api/Animals/{id}");
			}

			return RedirectToAction("Index");
		}
	}
}
