using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;

namespace ShelterHelper.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string _assignmentsEndpoint = "api/assignments/";

        public AssignmentsController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // GET: AssignmentsController
        public async Task<IActionResult> Index()
        {
            IEnumerable<Assignment> assignments = null;
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var response = await httpClient.GetAsync(_assignmentsEndpoint);

            if (response.IsSuccessStatusCode)
            {
                assignments = await response.Content.ReadAsAsync<IEnumerable<Assignment>>();
                assignments = assignments.OrderByDescending(a => a.Priority);
            }

            return View(assignments);
        }

        // GET: AssignmentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssignmentsController/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateConfirmed(Assignment assignment)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogDebug($"Posting new assignment: {assignment.Title} data to {_assignmentsEndpoint}.");
                    var response = await httpClient.PostAsJsonAsync(_assignmentsEndpoint, assignment);
                    response.EnsureSuccessStatusCode();
                    _logger.LogDebug(
                        $"Posted new assignment: {assignment.Title} data to {_assignmentsEndpoint} successfully");
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

            return View(assignment);
        }

        // GET: AssignmentsController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var assignment = new Assignment();
            if (id == null)
            {
                return NotFound();
            }

            HttpResponseMessage response = await httpClient.GetAsync($"{_assignmentsEndpoint}{id}");
            if (response.IsSuccessStatusCode)
            {
                assignment = await response.Content.ReadAsAsync<Assignment>();
            }

            return View(assignment);
        }

        // POST: AssignmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id,
            [Bind("AssignmentId, Title, Description, Priority, CreatorId, CreationDate, IsCompleted, IsInProgress")]
            Assignment assignment)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    HttpResponseMessage response =
                        await httpClient.PostAsJsonAsync($"{_assignmentsEndpoint}{assignment.AssignmentId}",
                            assignment);
                    response.EnsureSuccessStatusCode();
                    TempData["Success"] = "Edited successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                TempData["Error"] = "Failed to edit.";
            }

            return View(assignment);
        }

        // GET: AssignmentsController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var assignment = new Assignment();
            if (id == null)
            {
                return NotFound();
            }

            var response = await httpClient.GetAsync($"{_assignmentsEndpoint}{id}");
            if (response.IsSuccessStatusCode)
            {
                assignment = await response.Content.ReadAsAsync<Assignment>();
            }

            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: AssignmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var response = await httpClient.GetAsync($"{_assignmentsEndpoint}{id}");

            if (response.IsSuccessStatusCode)
            {
                await httpClient.DeleteAsync($"{_assignmentsEndpoint}{id}");
                TempData["Success"] = "Deleted successfully.";
                
            }
            return RedirectToAction("Index");
        }
    }
}