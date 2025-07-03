using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShelterHelper.Models;
using ShelterHelper.ViewModels;

namespace ShelterHelper.Controllers;

public class AccountController : Controller
{
    private readonly API.Controllers.EmployeesAssignmentsController _employeesAssignmentsController;

    public AccountController(API.Controllers.EmployeesAssignmentsController employeesAssignmentsController)
    {
        _employeesAssignmentsController = employeesAssignmentsController;
    }

    // GET
    public async Task LogIn(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    // GET
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var employee =
            new Employee(
                User.Claims.FirstOrDefault(c => c.Type == "https://localhost:7082/employee_name")?.Value,
                int.Parse(User.Claims.FirstOrDefault(c => c.Type == "https://localhost:7082/employee_personal_id")
                    ?.Value));
        var employeeAssignmentsList =
            await _employeesAssignmentsController.GetAssignmentsOfEmployee(employee.EmployeePersonalId.ToString());

        return View(new EmployeeViewModel(employee,
            employeeAssignmentsList.OrderByDescending(assignment => assignment.Assignment.Priority)));
    }

    // GET
    [Authorize]
    public async Task LogOut()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri(Url.Action("Welcome", "Home"))
            .Build();
        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}