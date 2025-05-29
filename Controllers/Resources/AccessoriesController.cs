using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers.Resources;

public class AccessoriesController : Controller
{
    private readonly ResourcesController _resourcesController;

    public AccessoriesController(ResourcesController resourcesController)
    {
        _resourcesController = resourcesController;
    }

    // POST: StorageController/Accessory
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateNewAccessory(SpeciesViewModel viewModel)
    {
        var accessory = new Accessory
        {
            AccessoryName = viewModel.Accessory.AccessoryName,
            Quantity = viewModel.Accessory.Quantity
        };

        if (ModelState.IsValid)
        {
            try
            {
                await _resourcesController.PostNewAccessory(accessory);
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

        return RedirectToAction("Index", "Storage");
    }

    public async Task<IActionResult> EditAccessory(int? id)
    {
        var accessory = new Accessory();

        if (id == null) return NotFound();

        try
        {
            var accessoryId = id ?? 0;
            var getAccessory = await _resourcesController.GetAccessory(accessoryId);
            accessory = getAccessory.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return View("/Views/Storage/EditResource/EditAccessory.cshtml", accessory);
    }

    // POST: StorageController/Resources/Accessories/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmEditAccessory(
        [Bind("AccessoryId", "AccessoryName", "Quantity")]
        Accessory accessory)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _resourcesController.PostNewAccessory(accessory);
                TempData["Success"] = "Success, database updated.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return RedirectToAction("Index", "Storage");
        }

        return View("/Views/Storage/EditResource/EditAccessory.cshtml", accessory);
    }

    // GET: StorageController/Resources/Accessories/5
    public async Task<IActionResult> DeleteAccessory(int? id)
    {
        var accessory = new Accessory();

        if (id == null) return NotFound();
        try
        {
            int accessoryId = id ?? 0;
            var getAccessory = await _resourcesController.GetAccessory(accessoryId);
            accessory = getAccessory.Value;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        if (accessory == null) return NotFound();
        return View("/Views/Storage/DeleteResource/DeleteAccessory.cshtml", accessory);
    }

    // POST: StorageController/Resources/Accessories/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeleteAccessory(int accessoryId)
    {
        try
        {
            await _resourcesController.DeleteAccessory(accessoryId);
            TempData["Success"] = "Deleted successfully.";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return RedirectToAction("Index", "Storage");
    }
}