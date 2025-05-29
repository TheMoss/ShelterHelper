using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers.Resources;

public class ToysController : Controller
{
    private readonly ResourcesController _resourcesController;

    public ToysController(ResourcesController resourcesController)
    {
        _resourcesController = resourcesController;
    }
   // POST: StorageController/Toy
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateNewToy(SpeciesViewModel viewModel)
    {
        var toy = new Toy
        {
            ToyName = viewModel.Toy.ToyName,
            Quantity = viewModel.Toy.Quantity
        };

        if (ModelState.IsValid)
        {
            try
            {
                await _resourcesController.PostNewToy(toy);
                TempData["Success"] = "Success, new toy type added.";
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

        return View("Views/Storage/Index.cshtml");
    }

    public async Task<IActionResult> EditToy(int? id)
    {
        var toy = new Toy();
        
        if (id == null) return NotFound();
        try
        {
            var toyId = id ?? 0;
            var getToy = await _resourcesController.GetToy(toyId);
            toy = getToy.Value;
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
        if (ModelState.IsValid)
        {
            try
            {
                
                await _resourcesController.PostNewToy(toy);
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Storage");
        }

        return View("Views/Storage/EditResource/EditToy.cshtml", toy);
    }


    // GET: StorageController/Resources/Toys/5
    public async Task<IActionResult> DeleteToy(int? id)
    {
        var toy = new Toy();
        if (id == null) return NotFound();
        try
        {
            var toyId = id ?? 0;
            var getToy = await _resourcesController.GetToy(toyId);
            toy = getToy.Value;
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
       try
       {
           await _resourcesController.DeleteToy(toyId);
       }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index", "Storage");
    }

}