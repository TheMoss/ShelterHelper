using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers.Resources;

public class BeddingsController : Controller
{
    private readonly ResourcesController _resourcesController;

    public BeddingsController(ResourcesController resourcesController)
    {
        _resourcesController = resourcesController;
    }
   // POST: StorageController/Bedding
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewBedding(SpeciesViewModel viewModel)
    {
       
        var bedding = new Bedding
        {
            BeddingName = viewModel.Bedding.BeddingName,
            Quantity_kg = viewModel.Bedding.Quantity_kg
        };

        if (ModelState.IsValid)
        {
            try
            {
                await _resourcesController.PostNewBedding(bedding);
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

        
        if (id == null) return NotFound();
        try
        {
            
            var beddingId = id ?? 0;
            var getBedding = await _resourcesController.GetBedding(beddingId);
            bedding = getBedding.Value;
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
        
        if (ModelState.IsValid)
        {
            try
            {
//TODO: fix edits so they use patch method
               
                await _resourcesController.PostNewBedding(bedding);
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Storage");
        }

        return View(bedding);
    }


    // GET: StorageController/Resources/Bedding/5
    public async Task<IActionResult> DeleteBedding(int? id)
    {
        var bedding = new Bedding();

        if (id == null) return NotFound();
        try
        {
            int beddingId = id ?? 0;
            var getBedding = await _resourcesController.GetBedding(beddingId);
            bedding = getBedding.Value;
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
        try
        {
            await _resourcesController.DeleteBedding(beddingId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index", "Storage");
    }

}