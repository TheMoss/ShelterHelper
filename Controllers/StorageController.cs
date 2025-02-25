using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

namespace ShelterHelper.Controllers;

public class StorageController : Controller
{
    public const string ResourcesEndpoint = "api/resources/";
    public const string SpeciesEndpoint = "api/species/";

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
        var resources = new SpeciesViewModel();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync(ResourcesEndpoint);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                resources = await response.Content.ReadAsAsync<SpeciesViewModel>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View(resources);
    }

    public async Task<IActionResult> Species(int? page, string sortOrder)
    {
        ViewBag.CurrentStorageSortOrder = sortOrder;
        IEnumerable<Species> species = null;

        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        try
        {
            _logger.LogDebug($"Requesting data from {SpeciesEndpoint}.");
            var response = await httpClient.GetAsync(SpeciesEndpoint);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {SpeciesEndpoint} received");
                species = await response.Content.ReadAsAsync<IEnumerable<Species>>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

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
        var viewModel = new SpeciesViewModel();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync(ResourcesEndpoint);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                var content = await response.Content.ReadAsStringAsync();
                viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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
        var viewModel = new SpeciesViewModel();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        var response = await httpClient.GetAsync(ResourcesEndpoint);
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                var content = await response.Content.ReadAsStringAsync();
                viewModel = JsonConvert.DeserializeObject<SpeciesViewModel>(content);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View(viewModel);
    }

    // POST: StorageController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SpeciesViewModel viewModel)
    {
        //id as value
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
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
                _logger.LogDebug(
                    $"Posting id: {species.SpeciesId}, species: {species.SpeciesName} data to {SpeciesEndpoint}.");
                var response = await httpClient.PostAsJsonAsync(SpeciesEndpoint, species);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted id:{species.SpeciesId}, species: {species.SpeciesName} data to {SpeciesEndpoint} successfully");
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
    public async Task<IActionResult> Edit(int? id)
    {
        var speciesViewModel = new SpeciesViewModel();
        var availableSpeciesOptions = new SpeciesViewModel();

        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        if (id == null) return NotFound();

        try
        {
            _logger.LogDebug($"Requesting data from {SpeciesEndpoint}.");
            var response = await httpClient.GetAsync($"{SpeciesEndpoint}{id}");

            //get all diets etc. from the resources but use current species data to preselect initial diet value
            if (response.IsSuccessStatusCode)
            {
                speciesViewModel = await response.Content.ReadAsAsync<SpeciesViewModel>();
                _logger.LogDebug($"Data from {SpeciesEndpoint} received.");

                var responseSpeciesOptions = await httpClient.GetAsync(ResourcesEndpoint);

                if (responseSpeciesOptions.IsSuccessStatusCode)
                {
                    availableSpeciesOptions = await responseSpeciesOptions.Content.ReadAsAsync<SpeciesViewModel>();
                }
                else
                {
                    return NotFound();
                }


                speciesViewModel.AccessoriesList = availableSpeciesOptions.AccessoriesList;
                speciesViewModel.BeddingsList = availableSpeciesOptions.BeddingsList;
                speciesViewModel.DietsList = availableSpeciesOptions.DietsList;
                speciesViewModel.ToysList = availableSpeciesOptions.ToysList;

                speciesViewModel.SelectedAccessoryId = speciesViewModel.Accessory.AccessoryId;
                speciesViewModel.SelectedBeddingId = speciesViewModel.Bedding.BeddingId;
                speciesViewModel.SelectedDietId = speciesViewModel.Diet.DietId;
                speciesViewModel.SelectedToyId = speciesViewModel.Toy.ToyId;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/Species/Edit.cshtml", speciesViewModel);
    }

    //POST : StorageController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditSpecies(int id,
        [Bind("SpeciesId", "SpeciesName", "SelectedDietId", "SelectedBeddingId", "SelectedToyId",
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
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogDebug(
                    $"Posting data to {SpeciesEndpoint}, id: {speciesViewModel.SpeciesId}, species name: {speciesViewModel.SpeciesName}.");
                var response =
                    await httpClient.PostAsJsonAsync($"{SpeciesEndpoint}{speciesUpdate.SpeciesId}", speciesUpdate);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug($"Posted {speciesViewModel.SpeciesName} data to {SpeciesEndpoint} successfully");
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Species");
        }

        return View(speciesUpdate);
    }


    // GET: StorageController/Species/5
    public async Task<IActionResult> Delete(int? id)
    {
        var species = new Species();

        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        if (id == null) return NotFound();

        try
        {
            _logger.LogDebug($"Requesting data from {SpeciesEndpoint}.");
            var response = await httpClient.GetAsync($"{SpeciesEndpoint}{id}");
            if (response.IsSuccessStatusCode)
            {
                species = await response.Content.ReadAsAsync<Species>();
            }

            _logger.LogDebug($"Data from {SpeciesEndpoint} received.");
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
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        try
        {
            _logger.LogDebug($"Requesting deleting data from {SpeciesEndpoint}, id: {speciesId}.");
            var response = await httpClient.GetAsync($"{SpeciesEndpoint}{speciesId}");

            if (response.IsSuccessStatusCode)
            {
                await httpClient.DeleteAsync($"{SpeciesEndpoint}{speciesId}");
                _logger.LogDebug($"Deleted data from {SpeciesEndpoint} successfully, id: {speciesId}.");
                TempData["Success"] = "Deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Species");
    }


    #region Accessory

    // POST: StorageController/Accessory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewAccessory(SpeciesViewModel viewModel)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        var accessory = new Accessory
        {
            AccessoryName = viewModel.Accessory.AccessoryName,
            Quantity = viewModel.Accessory.Quantity
        };

        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogDebug(
                    $"Posting data to {ResourcesEndpoint}, id:{accessory.AccessoryId}, accessory name: {accessory.AccessoryName}.");
                var response = await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}accessories", accessory);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted data to {ResourcesEndpoint} successfully, id:{accessory.AccessoryId}, accessory name: {accessory.AccessoryName}.");
                TempData["Success"] = "Success, new accessory type added.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        else
        {
            TempData["Error"] = "Error, ModelState invalid.";
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditAccessory(int? id)
    {
        var accessory = new Accessory();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (id == null) return NotFound();

        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}accessories/{id}");
            if (response.IsSuccessStatusCode)
            {
                accessory = await response.Content.ReadAsAsync<Accessory>();
                _logger.LogDebug($"Data from {ResourcesEndpoint} received.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/EditResource/EditAccessory.cshtml", accessory);
    }

    // POST: StorageController/Resources/Accessories/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditAccessory(
        [Bind("AccessoryId", "AccessoryName", "Quantity")]
        Accessory accessory)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogDebug(
                    $"Posting data to {ResourcesEndpoint}, id:{accessory.AccessoryId}, accessory name: {accessory.AccessoryName}.");
                var response =
                    await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}accessories/{accessory.AccessoryId}",
                        accessory);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted data to {ResourcesEndpoint} successfully, id:{accessory.AccessoryId}, accessory name: {accessory.AccessoryName}.");
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index");
        }

        return View(accessory);
    }

    // GET: StorageController/Resources/Accessories/5
    public async Task<IActionResult> DeleteAccessory(int? id)
    {
        var accessory = new Accessory();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}accessories/{id}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                accessory = await response.Content.ReadAsAsync<Accessory>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        if (accessory == null) return NotFound();
        return View("Views/Storage/DeleteResource/DeleteAccessory.cshtml", accessory);
    }

    // POST: StorageController/Resources/Accessories/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeleteAccessory(int accessoryId)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}accessories/{accessoryId}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                _logger.LogDebug($"Deleting data from {ResourcesEndpoint}, id: {accessoryId}.");
                await httpClient.DeleteAsync($"{ResourcesEndpoint}accessories/{accessoryId}");
                _logger.LogDebug($"Deleted data from {ResourcesEndpoint} successfully, id:{accessoryId}");
                TempData["Success"] = "Deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        return RedirectToAction("Index");
    }

    #endregion Accessory


    #region Bedding

    // POST: StorageController/Bedding
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewBedding(SpeciesViewModel viewModel)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        var bedding = new Bedding
        {
            BeddingName = viewModel.Bedding.BeddingName,
            Quantity_kg = viewModel.Bedding.Quantity_kg
        };

        if (ModelState.IsValid)
        {
            try
            {
                //check if entry exists or create a new one
                _logger.LogDebug(
                    $"Posting data to {ResourcesEndpoint}, name: {bedding.BeddingName}, quantity: {bedding.Quantity_kg}.");
                var response = await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}beddings", bedding);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug($"Posted data to {ResourcesEndpoint} successfully");
                TempData["Success"] = "Success, new bedding type added.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        else
        {
            TempData["Error"] = "Error, something went wrong.";
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditBedding(int? id)
    {
        var bedding = new Bedding();

        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}beddings/{id}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                bedding = await response.Content.ReadAsAsync<Bedding>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/EditResource/EditBedding.cshtml", bedding);
    }

    // POST: StorageController/Resources/Bedding/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditBedding(
        [Bind("BeddingId", "BeddingName", "Quantity_kg")]
        Bedding bedding)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogDebug(
                    $"Posting data to {ResourcesEndpoint}, id:{bedding.BeddingId}, name: {bedding.BeddingName}, quantity: {bedding.Quantity_kg}.");
                var response =
                    await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}beddings/{bedding.BeddingId}", bedding);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted data to {ResourcesEndpoint} successfully, id:{bedding.BeddingId}, name: {bedding.BeddingName}, quantity: {bedding.Quantity_kg}.");
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index");
        }

        return View(bedding);
    }


    // GET: StorageController/Resources/Bedding/5
    public async Task<IActionResult> DeleteBedding(int? id)
    {
        var bedding = new Bedding();

        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}beddings/{id}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                bedding = await response.Content.ReadAsAsync<Bedding>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        if (bedding == null) return NotFound();
        return View("Views/Storage/DeleteResource/DeleteBedding.cshtml", bedding);
    }

    // POST: StorageController/Resources/Bedding/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeleteBedding(int beddingId)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}beddings/{beddingId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received");
                await httpClient.DeleteAsync($"{ResourcesEndpoint}beddings/{beddingId}");
                _logger.LogDebug($"Data from {ResourcesEndpoint} deleted, id: {beddingId}");
                TempData["Success"] = "Deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index");
    }

    #endregion Bedding


    #region Diet

    // POST: StorageController/Diet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewDiet(SpeciesViewModel viewModel)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        var diet = new Diet
        {
            DietName = viewModel.Diet.DietName,
            Quantity_kg = viewModel.Diet.Quantity_kg
        };

        try
        {
            if (ModelState.IsValid)
            {
                //check if entry exists or create a new one
                _logger.LogDebug(
                    $"Posting data to {ResourcesEndpoint}, name: {diet.DietName}, quantity: {diet.Quantity_kg}.");
                var response = await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}diets", diet);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted data to {ResourcesEndpoint} successfully, name: {diet.DietName}, quantity: {diet.Quantity_kg}.");
                TempData["Success"] = "Success, new diet type added.";
            }
            else
            {
                TempData["Error"] = "Error, something went wrong.";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditDiet(int? id)
    {
        var diet = new Diet();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}, id: {id}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}diets/{id}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received, id: {id}");
                diet = await response.Content.ReadAsAsync<Diet>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/EditResource/EditDiet.cshtml", diet);
    }

    // POST: StorageController/Resources/Diets/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditDiet([Bind("DietId", "DietName", "Quantity_kg")] Diet diet)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogDebug(
                    $"Posting data to {ResourcesEndpoint}, id: {diet.DietId}, name:{diet.DietName}, quantity: {diet.Quantity_kg}.");
                var response = await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}diets/{diet.DietId}", diet);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted data to {SpeciesEndpoint} successfully, id: {diet.DietId}, name:{diet.DietName}, quantity: {diet.Quantity_kg}.");
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
        var diet = new Diet();
        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}, id: {id}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}diets/{id}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received, id: {id}");
                diet = await response.Content.ReadAsAsync<Diet>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/DeleteResource/DeleteDiet.cshtml", diet);
    }

    // POST: StorageController/Resources/Diets/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeleteDiet(int dietId)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
        var response = await httpClient.GetAsync($"{ResourcesEndpoint}diets/{dietId}");
        try
        {
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received.");
                await httpClient.DeleteAsync($"{ResourcesEndpoint}diets/{dietId}");
                _logger.LogDebug($"Deleted data from {ResourcesEndpoint}, id: {dietId}");
                TempData["Success"] = "Deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index");
    }

    #endregion Diet


    #region Toy

    // POST: StorageController/Toy
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateNewToy(SpeciesViewModel viewModel)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        var toy = new Toy
        {
            ToyName = viewModel.Toy.ToyName,
            Quantity = viewModel.Toy.Quantity
        };

        if (ModelState.IsValid)
        {
            //check if entry exists or create a new one
            _logger.LogDebug($"Posting data to {ResourcesEndpoint}, name: {toy.ToyName}, quantity: {toy.Quantity}.");
            var response = await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}toys", toy);
            response.EnsureSuccessStatusCode();
            _logger.LogDebug(
                $"Posted data to {ResourcesEndpoint} successfully, name: {toy.ToyName}, quantity: {toy.Quantity}.");
            TempData["Success"] = "Success, new toy type added.";
        }
        else
        {
            TempData["Error"] = "Error, something went wrong.";
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditToy(int? id)
    {
        var toy = new Toy();
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}toys/{id}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received.");
                toy = await response.Content.ReadAsAsync<Toy>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/EditResource/EditToy.cshtml", toy);
    }

    // POST: StorageController/Resources/Toys/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditToy([Bind("ToyId", "ToyName", "Quantity")] Toy toy)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogDebug(
                    $"Posting data to {SpeciesEndpoint}, id:{toy.ToyId}, name: {toy.ToyName}, quantity: {toy.Quantity}.");
                var response = await httpClient.PostAsJsonAsync($"{ResourcesEndpoint}toys/{toy.ToyId}", toy);
                response.EnsureSuccessStatusCode();
                _logger.LogDebug(
                    $"Posted data to {ResourcesEndpoint} successfully, id:{toy.ToyId}, name: {toy.ToyName}, quantity: {toy.Quantity}.");
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
        var toy = new Toy();
        if (id == null) return NotFound();
        try
        {
            _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
            var response = await httpClient.GetAsync($"{ResourcesEndpoint}toys/{id}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received.");
                toy = await response.Content.ReadAsAsync<Toy>();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Storage/DeleteResource/DeleteToy.cshtml", toy);
    }

    // POST: StorageController/Resources/Toys/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeleteToy(int toyId)
    {
        var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
        _logger.LogDebug($"Requesting data from {ResourcesEndpoint}.");
        var response = await httpClient.GetAsync($"{ResourcesEndpoint}toys/{toyId}");
        try
        {
            if (response.IsSuccessStatusCode)
            {
                _logger.LogDebug($"Data from {ResourcesEndpoint} received.");
                await httpClient.DeleteAsync($"{ResourcesEndpoint}toys/{toyId}");
                _logger.LogDebug($"Deleted data from {ResourcesEndpoint}, id: {toyId}");
                TempData["Success"] = "Deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to delete";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index");
    }

    #endregion Toy
}