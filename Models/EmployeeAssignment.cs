
using System.ComponentModel.DataAnnotations;

namespace ShelterHelper.Models;

public class EmployeeAssignment
{
    [Key]
    public int? Id { get; set; }
    
    public int EmployeeId { get; set; }
    public int? AssignmentId { get; set; }

    public Employee Employee { get; set; }
    public Assignment Assignment { get; set; }
}