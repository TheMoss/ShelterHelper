using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using System.Diagnostics;
using System.Net.Http.Headers;
using X.PagedList;

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

		public async Task<ActionResult> Index(int? page, string sortOrder)
		{
			ViewBag.CurrentAnimalSortOrder = sortOrder;
			IEnumerable<Animal> animals = null;
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Animals");
			
			if (response.IsSuccessStatusCode)
			{
				animals = await response.Content.ReadAsAsync<IEnumerable<Animal>>();
			}

			ViewBag.AnimalsSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.SexParam = sortOrder == "Sex" ? "sex_desc" : "Sex";
			ViewBag.AdmissionParam = sortOrder == "AdmissionDate" ? "admission_date_desc" : "AdmissionDate";
			ViewBag.AdoptionParam = sortOrder == "AdoptionDate" ? "adoption_date_desc" : "AdoptionDate";
			ViewBag.EmployeeParam = sortOrder == "Employee" ? "employee_desc" : "Employee";
			switch(sortOrder)
			{
				case "name_desc":
					animals = animals.OrderByDescending(a => a.Name);
					break;
				case "Sex":
					animals = animals.OrderBy(a => a.Sex);
					break;
				case "sex_desc":
					animals = animals.OrderByDescending(a => a.Sex);
					break;
				case "AdmissionDate":
					animals = animals.OrderBy(a => a.AdmissionDay);
					break;
				case "admission_date_desc":
					animals = animals.OrderByDescending(a => a.AdmissionDay);
					break;
				case "AdoptionDate":
					animals = animals.OrderBy(a => a.AdoptionDay);
					break;
				case "adoption_date_desc":
					animals = animals.OrderByDescending(a => a.AdoptionDay);
					break;
				case "Employee":
					animals = animals.OrderBy(a => a.EmployeeId);
					break;
				case "employee_desc":
					animals = animals.OrderByDescending(a=>a.EmployeeId);
					break;
				default:
					animals = animals.OrderBy(a => a.Name);
					break;
			}
			var pageNumber = page ?? 1;
			var pagedList = animals.ToPagedList(pageNumber, 10);
			ViewBag.AnimalsPagedList = pagedList;
			return View(animals);
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public ActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task<ActionResult> Create()
		{
			var httpClient = _httpClientFactory.CreateClient("Client");
			HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7147/api/Species");
			IEnumerable<Species> species = null;
			var viewModel = new AnimalViewModel();
			if (response.IsSuccessStatusCode)
			{				
				species = await response.Content.ReadAsAsync<IEnumerable<Species>>();

				viewModel.SpeciesList = species.ToList();
			}

			return View(viewModel);
		}

		[HttpPost, ActionName("Create")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateConfirmed([Bind("Id, Species, Name, Sex, Weight, AdmissionDay, AdoptionDay, Health, EmployeeId")] AnimalViewModel animal)
		{
			var httpClient = _httpClientFactory.CreateClient("Client");

			animal.Animal.AdoptionDay = new DateOnly(1900, 1, 1);
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
