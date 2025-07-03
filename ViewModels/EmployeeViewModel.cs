using ShelterHelper.Models;

namespace ShelterHelper.ViewModels;

public class EmployeeViewModel
{
    public Employee? Employee { get; set; }
    public EmployeeAssignment? EmployeeAssignment { get; set; }
    
    public IEnumerable<EmployeeAssignment>? EmployeeAssignmentsList { get; set; }
    public EmployeeViewModel(Employee employee, IEnumerable<EmployeeAssignment> employeeAssignmentsList)
    {
        Employee = employee;
        EmployeeAssignmentsList = employeeAssignmentsList;
    }
}