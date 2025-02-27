using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string _assignmentsEndpoint = "api/assignments/";
        private const string _employeesEndpoint = "api/employee";
        private const string _employeesAssignmentsEndpoint = "api/EmployeesAssignments";

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
        public async Task<ActionResult> Create()
        {
            var assignmentViewModel = new AssignmentViewModel();
            return View(assignmentViewModel);
        }

        // POST: AssignmentsController/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateConfirmed(AssignmentViewModel assignmentViewModel)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogDebug(
                        $"Posting new assignment: {assignmentViewModel.Assignment.Title} data to {_assignmentsEndpoint}.");
                    var assignmentResponse =
                        await httpClient.PostAsJsonAsync(_assignmentsEndpoint, assignmentViewModel.Assignment);
                    assignmentResponse.EnsureSuccessStatusCode();
                    _logger.LogDebug(
                        $"Posted new assignment: {assignmentViewModel.Assignment.Title} data to {_assignmentsEndpoint} successfully");
                    TempData["Success"] = "Success, database updated.";
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Error, something went wrong. Check the console for more details";
                    Console.WriteLine(e);
                }

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Error, ModelState invalid.";

            return View(assignmentViewModel);
        }

        // GET: AssignmentsController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");
            var assignment = new Assignment();
            var assignmentViewModel = new AssignmentViewModel();
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var response = await httpClient.GetAsync($"{_assignmentsEndpoint}{id}");
                if (response.IsSuccessStatusCode)
                {
                    assignment = await response.Content.ReadAsAsync<Assignment>();
                }

                var getAllEmployeesResponse = await httpClient.GetAsync(_employeesEndpoint);
                if (getAllEmployeesResponse.IsSuccessStatusCode)
                {
                    IEnumerable<Employee> allEmployees =
                        await getAllEmployeesResponse.Content.ReadAsAsync<IEnumerable<Employee>>();

                    if (allEmployees != null)
                    {
                        assignmentViewModel.EmployeesList = allEmployees.ToList();
                    }
                }
                
                assignmentViewModel.Assignment = assignment;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View(assignmentViewModel);
        }

        // POST: AssignmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] AssignmentViewModel assignmentViewModel)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

            if (ModelState.IsValid)
            {
                try
                {
                    //post edited assignment
                    var editedAssignment = new Assignment()
                    {
                        AssignmentId = assignmentViewModel.Assignment.AssignmentId,
                        CreationDate = assignmentViewModel.Assignment.CreationDate,
                        CreatorId = assignmentViewModel.Assignment.CreatorId,
                        Description = assignmentViewModel.Assignment.Description,
                        IsCompleted = assignmentViewModel.Assignment.IsCompleted,
                        IsInProgress = assignmentViewModel.Assignment.IsInProgress,
                        Priority = assignmentViewModel.Assignment.Priority,
                        Title = assignmentViewModel.Assignment.Title
                    };
                    var response = await httpClient.PostAsJsonAsync($"{_assignmentsEndpoint}{assignmentViewModel.Assignment.AssignmentId}", editedAssignment);
                    response.EnsureSuccessStatusCode();

                    if (assignmentViewModel.SelectedEmployeesIds.Count != null)
                    {
                        foreach (var employeeId in assignmentViewModel.SelectedEmployeesIds)
                        {
                            var employeeAssignment = new EmployeesAssignments()
                            {
                                AssignmentId = assignmentViewModel.Assignment.AssignmentId,
                                EmployeeId = employeeId
                            };
                            var employeesAssignmentsResponse =
                                await httpClient.PostAsJsonAsync($"{_employeesAssignmentsEndpoint}",
                                    employeeAssignment);
                            employeesAssignmentsResponse.EnsureSuccessStatusCode();
                            _logger.LogInformation("Successfully posted {employeeAssignment} to EmployeesAssignments table", employeeAssignment);
                        }
                        
                    }

                    else {
                        //TODO: post delete no longer assigned people 
                        //get all pairs assigned to task, delete those not mentioned
                        //or ?
                    }

                    TempData["Success"] = "Edited successfully.";
                    
                }
                catch (Exception e)
                {
                    TempData["Error"] = "Failure, check the console for details.";
                    Console.WriteLine(e);
                }
            }
            else
            {
                TempData["Error"] = "Failed to edit.";
            }
            return RedirectToAction("Index");
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