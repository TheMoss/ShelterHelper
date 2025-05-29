using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using X.PagedList.Extensions;

namespace ShelterHelper.Controllers;

public class StorageController : Controller
{
   
    private readonly ILogger _logger;
    private readonly ResourcesController _resourcesController;
    private readonly SpeciesController _speciesController;

    public StorageController(ILogger<StorageController> logger,
        ResourcesController resourcesController, SpeciesController speciesController)
    {
        _logger = logger;
        _resourcesController = resourcesController;
        _speciesController = speciesController;
    }

    // GET: StorageController
    public async Task<IActionResult> Index()
    {
        var resourcesDto = await _resourcesController.Get();
        var resources = new SpeciesViewModel();
        if (resourcesDto.Value is not null)
        {
            resources.AccessoriesList = resourcesDto.Value.AccessoriesList;
            resources.BeddingsList = resourcesDto.Value.BeddingsList;
            resources.DietsList = resourcesDto.Value.DietsList;
            resources.ToysList = resourcesDto.Value.ToysList;
        }
        
        return View(resources);
    }

    public async Task<IActionResult> Species(int? page, string sortOrder)
    {
        ViewBag.CurrentStorageSortOrder = sortOrder;
        var species = await _speciesController.GetSpeciesDb();

        ViewBag.StorageSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

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
        // var viewModel = new SpeciesViewModel();
        //
        // var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        // try
        // {
        //     var response = await httpClient.GetAsync(ResourcesEndpoint);
        //     if (response.IsSuccessStatusCode)
        //     {
        //         var content = await response.Content.ReadAsStringAsync();
        //         viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
        //     }
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        // }
        var resourcesDto = await _resourcesController.Get();
        var resources = new SpeciesViewModel();
        if (resourcesDto.Value is not null)
        {
            resources.AccessoriesList = resourcesDto.Value.AccessoriesList;
            resources.BeddingsList = resourcesDto.Value.BeddingsList;
            resources.DietsList = resourcesDto.Value.DietsList;
            resources.ToysList = resourcesDto.Value.ToysList;
        }

        ;
        return View("Views/Storage/Species/Create.cshtml", resources);
    }

    // GET: StorageController/Details/5
    public IActionResult Details(int id)
    {
        return View();
    }

    public async Task<IActionResult> AddResources()
    {
        // var viewModel = new SpeciesViewModel();
        //
        // var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        //
        // var response = await httpClient.GetAsync(ResourcesEndpoint);
        // try
        // {
        //     
        //     if (response.IsSuccessStatusCode)
        //     {
        //         
        //         var content = await response.Content.ReadAsStringAsync();
        //         viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
        //     }
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        // }
        var resourcesDto = await _resourcesController.Get();
        var resources = new SpeciesViewModel();
        if (resourcesDto.Value is not null)
        {
            resources.AccessoriesList = resourcesDto.Value.AccessoriesList;
            resources.BeddingsList = resourcesDto.Value.BeddingsList;
            resources.DietsList = resourcesDto.Value.DietsList;
            resources.ToysList = resourcesDto.Value.ToysList;
        }

        ;
        return View(resources);
    }

    // POST: StorageController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SpeciesViewModel viewModel)
    {
        //id as value

        //var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        var species = new Species
        {
            SpeciesName = viewModel.Species.SpeciesName,
            DietId = viewModel.SelectedDietId,
            BeddingId = viewModel.SelectedBeddingId,
            ToyId = viewModel.SelectedToyId,
            AccessoryId = viewModel.SelectedAccessoryId
        };


        if (ModelState.IsValid)
        {
            try
            {

                //var response = await httpClient.PostAsJsonAsync(SpeciesEndpoint, species);
                await _speciesController.PostSpecies(species);
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index");
        }
        else
        {
            TempData["Error"] = "Error, ModelState invalid.";
        }

        return View(species);
    }

    //GET : StorageController/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var speciesData = await _speciesController.GetSpecies(id);
        var speciesOptions = await _resourcesController.Get();
        var speciesViewModel = new SpeciesViewModel()
        {
            SpeciesId = id,
            SpeciesName = speciesData.Value.SpeciesName,

            AccessoriesList = speciesOptions.Value.AccessoriesList,
            BeddingsList = speciesOptions.Value.BeddingsList,
            DietsList = speciesOptions.Value.DietsList,
            ToysList = speciesOptions.Value.ToysList,

            SelectedAccessoryId = speciesData.Value.Accessory.AccessoryId,
            SelectedBeddingId = speciesData.Value.Bedding.BeddingId,
            SelectedDietId = speciesData.Value.Diet.DietId,
            SelectedToyId = speciesData.Value.Toy.ToyId
        };

        return View("Views/Storage/Species/Edit.cshtml", speciesViewModel);
    }

    //POST : StorageController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditSpecies([Bind("SpeciesId", "SpeciesName", "SelectedDietId",
            "SelectedBeddingId", "SelectedToyId",
            "SelectedAccessoryId")]
        SpeciesViewModel speciesViewModel)
    {
        var speciesUpdate = new Species
        {
            SpeciesId = speciesViewModel.SpeciesId,
            SpeciesName = speciesViewModel.SpeciesName,
            DietId = speciesViewModel.SelectedDietId,
            BeddingId = speciesViewModel.SelectedBeddingId,
            ToyId = speciesViewModel.SelectedToyId,
            AccessoryId = speciesViewModel.SelectedAccessoryId
        };
        var id = speciesUpdate.SpeciesId ?? 0;
        if (ModelState.IsValid && id != 0)
        {
            try
            {
                await _speciesController.PutSpecies(id, speciesUpdate);

                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Species");
        }

        TempData["Error"] = "Error, ModelState invalid.";
        return View("Views/Storage/Species/Edit.cshtml");
    }


    // GET: StorageController/Species/5
    public async Task<IActionResult> Delete(int? id)
    {
        var species = new Species();

        if (id == null) return NotFound();

        try
        {
            int speciesId = id ?? 0;
            var getSpecies = await _speciesController.GetSpecies(speciesId);
            species = getSpecies.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        return View("Views/Storage/Species/Delete.cshtml", species);
    }

    // POST: StorageController/Species/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDelete(int speciesId)
    {
        try
        {
            //TODO: fix foreign key modification error
            await _speciesController.DeleteSpecies(speciesId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Species");
    }
}