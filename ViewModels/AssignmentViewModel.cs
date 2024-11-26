using ShelterHelper.Models;

namespace ShelterHelper.ViewModels;

public class AssignmentViewModel
{
    public Assignment? Assignment { get; set; }
    public Employee? Employee { get; set; }

    public List<Employee>? EmployeesList { get; set; } = new();
    public List<int>? SelectedEmployeesIds { get; set; }
}