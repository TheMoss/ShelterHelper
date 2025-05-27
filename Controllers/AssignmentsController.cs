using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.API.Controllers;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers
{
    public class AssignmentsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly API.Controllers.AssignmentsController _assignmentsController;
        private readonly EmployeeController _employeesController;
        private readonly EmployeesAssignmentsController _employeesAssignmentsController;

        public AssignmentsController(ILogger<HomeController> logger, API.Controllers.AssignmentsController assignmentsController, EmployeeController employeesController, EmployeesAssignmentsController employeesAssignmentsController)
        {
            _logger = logger;
            _assignmentsController = assignmentsController;
            _employeesController = employeesController;
            _employeesAssignmentsController = employeesAssignmentsController;
        }

        // GET: AssignmentsController
        public async Task<IActionResult> Index()
        {
            var assignments = await _assignmentsController.GetAssignments();
            assignments = assignments.OrderByDescending(a => a.Priority);

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
                _assignmentsController.PostAssignment(assignmentViewModel.Assignment);
                TempData["Success"] = "New assignment added to the database.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Error, ModelState invalid.";

            return View(assignmentViewModel);
        }

        // GET: AssignmentsController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var assignmentViewModel = new AssignmentViewModel();
            if (id == null)
            {
                return BadRequest();
            }

            var assignment = await _assignmentsController.GetAssignment(id);
            if (assignment.Value == null)
            {
                return NotFound();
            }

            var allEmployees = await _employeesController.GetEmployees();
            if (allEmployees.Value == null)
            {
                return NotFound();
            }

            assignmentViewModel.EmployeesList = allEmployees.Value.ToList();
            assignmentViewModel.Assignment = assignment.Value;
            return View(assignmentViewModel);
        }

        // POST: AssignmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] AssignmentViewModel assignmentViewModel)
        {
            var httpClient = _httpClientFactory.CreateClient("ShelterHelperAPI");

            var originalAssignedEmployeesList = new List<Employee>();
            var originalAssignedEmployeesResponse =
                await httpClient.GetAsync(
                    $"{_employeesAssignmentSearchEndpoint}{assignmentViewModel.Assignment.AssignmentId}");
            if (originalAssignedEmployeesResponse.IsSuccessStatusCode)
            {
                originalAssignedEmployeesList = await originalAssignedEmployeesResponse.Content.ReadAsAsync<List<Employee>>();
            }
            //got old list of employees
            if (ModelState.IsValid)
            {
                try
                {
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
                    
                    await httpClient.PutAsJsonAsync($"{_employeesAssignmentsEndpoint}{editedAssignment.AssignmentId}", editedAssignment);
                    var originalEmployeesIds = originalAssignedEmployeesList.Select(e => e.EmployeeId);
                    //find new people
                    var newlyAssigned = assignmentViewModel.SelectedEmployeesIds.Except(originalEmployeesIds);
                    if (newlyAssigned.Count() > 0)
                    {
                        foreach (var employeeId in newlyAssigned)
                        {
                            var newEmployeeAssignment = new EmployeeAssignment()
                            {
                                EmployeeId = employeeId,
                                AssignmentId = assignmentViewModel.Assignment.AssignmentId
                            };
                            
                            await httpClient.PostAsJsonAsync(_employeesAssignmentsEndpoint, newEmployeeAssignment);
                        }
                    }

                    var noLongerAssigned = originalAssignedEmployeesList.Select(e => e.EmployeeId)
                        .Except(assignmentViewModel.SelectedEmployeesIds);
                    //find who to delete
                    if (noLongerAssigned.Count() > 0)
                    {
                        //to delete I need: assignment id, employee id, record id
                        //assignmentViewModel.Assignment.AssignmentId
                        //foreach employeeId in noLongerAssigned
                        var currentEmployeeAssignmentsResponse = await httpClient.GetAsync($"{_employeesAssignmentSearchEndpoint}{
                            assignmentViewModel.Assignment.AssignmentId}");//whole employeeassignment objects of current assignment
                        
                        if (currentEmployeeAssignmentsResponse.IsSuccessStatusCode)
                        {
                            var currentEmployeeAssignmentsList = await currentEmployeeAssignmentsResponse.Content
                                .ReadAsAsync<List<EmployeeAssignment>>();

                            foreach (var employeeId in noLongerAssigned)
                            {
                                var match = currentEmployeeAssignmentsList.Find(a => a.EmployeeId == employeeId);
                                if (match != null) await httpClient.DeleteAsync($"{_employeesAssignmentsEndpoint}{match.Id.ToString()}");
                            }
                            //which record ids match deleted employees
                        }
                    }
                    
                   
                    TempData["Success"] = "Edited successfully.";
                    return RedirectToAction("Index");
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
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var assignment = await _assignmentsController.GetAssignment(id);

            if (assignment.Value == null)
            {
                return NotFound();
            }

            return View(assignment.Value);
        }

        // POST: AssignmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _assignmentsController.DeleteAssignment(id);
            TempData["Success"] = "Deleted successfully.";
            
            return RedirectToAction("Index");
        }
    }
}