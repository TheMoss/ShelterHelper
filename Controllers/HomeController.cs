using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

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

	}
}
