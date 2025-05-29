using Microsoft.AspNetCore.Authorization;
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

        public AssignmentsController(ILogger<HomeController> logger,
            API.Controllers.AssignmentsController assignmentsController, EmployeeController employeesController,
            EmployeesAssignmentsController employeesAssignmentsController)
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
                    await _assignmentsController.PostSelectedAssignment(editedAssignment.AssignmentId,
                        editedAssignment);

                    var originalAssignedEmployees =
                        await _employeesAssignmentsController.GetEmployeesFilteredByAssignment(assignmentViewModel
                            .Assignment.AssignmentId.ToString());

                    var idsOriginal = originalAssignedEmployees?.Select(e => e.EmployeeId).ToList();
                    var idsNew = assignmentViewModel.SelectedEmployeesIds?.ExceptBy(idsOriginal, id => id).ToList();
                    
                    if (idsNew is not null)
                    {
                        foreach (var id in idsNew)
                        {
                            var employeeAssignment = new EmployeeAssignment
                            {
                                AssignmentId = assignmentViewModel.Assignment.AssignmentId,
                                EmployeeId = id
                            };
                            _employeesAssignmentsController.PostEmployeesAssignments(employeeAssignment);
                        }
                    }

                    await EmployeesRemovedFromAssignment(assignmentViewModel.Assignment.AssignmentId, idsOriginal,
                        assignmentViewModel.SelectedEmployeesIds);


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

        private async Task EmployeesRemovedFromAssignment(int? assignmentId, List<int>? idsOriginal,
            List<int>? currentIds)
        {
            try
            {
                if (currentIds is not null)
                {
                    var idsToRemove = idsOriginal.ExceptBy(currentIds, id => id);
                    foreach (var id in idsToRemove)
                    {
                        var queryResults =
                            await _employeesAssignmentsController.GetEmployeeAssignmentByAssignmentAndEmployeeIds(
                                currentIds.ToString(), id.ToString());
                        await _employeesAssignmentsController.DeleteEmployeeAssignment(queryResults[0].Id);
                    }
                }
                else
                {
                    if (idsOriginal is not null)
                    {
                        foreach (var id in idsOriginal)
                        {
                            var queryResults =
                                await _employeesAssignmentsController.GetEmployeeAssignmentByAssignmentAndEmployeeIds(
                                    assignmentId.ToString(), id.ToString());
                            Console.WriteLine(queryResults.First().Id);
                            await _employeesAssignmentsController.DeleteEmployeeAssignment(queryResults.First().Id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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