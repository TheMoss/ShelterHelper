using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers.Resources;

public class DietsController : Controller
{
    private readonly API.Controllers.ResourcesController _resourcesController;

    public DietsController(API.Controllers.ResourcesController resourcesController)
    {
        _resourcesController = resourcesController;
    }
   // POST: StorageController/Diet
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewDiet(SpeciesViewModel viewModel)
    {
        var diet = new Diet
        {
            DietName = viewModel.Diet.DietName,
            Quantity_kg = viewModel.Diet.Quantity_kg
        };

        try
        {
            if (ModelState.IsValid)
            {
                await _resourcesController.PostNewDiet(diet);
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

        return View("Views/Resources/Index.cshtml");
    }

    public async Task<IActionResult> EditDiet(int? id)
    {
        var diet = new Diet();
         if (id == null) return NotFound();
        try
        {
            var dietId = id ?? 0;
            var getDiet = await _resourcesController.GetDiet(dietId);
            diet = getDiet.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("Views/Resources/EditResource/EditDiet.cshtml", diet);
    }

    // POST: StorageController/Resources/Diets/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditDiet([Bind("DietId", "DietName", "Quantity_kg")] Diet diet)
    {
        if (ModelState.IsValid)
        {
            try
            {
               
                await _resourcesController.PostNewDiet(diet);
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Resources");
        }

        return View("Views/Resources/EditResource/EditDiet.cshtml", diet);
    }


    // GET: StorageController/Resources/Diets/5
    public async Task<IActionResult> DeleteDiet(int? id)
    {
        var diet = new Diet();
        if (id == null) return NotFound();
        try
        {
            int dietId = id ?? 0;
            var getDiet = await _resourcesController.GetDiet(dietId);
            diet = getDiet.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        if (diet == null) return NotFound();
        return View("Views/Resources/DeleteResource/DeleteDiet.cshtml", diet);
    }

    // POST: StorageController/Resources/Diets/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeleteDiet(int dietId)
    {
        try
        {
            await _resourcesController.DeleteDiet(dietId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index", "Resources");
    }

}