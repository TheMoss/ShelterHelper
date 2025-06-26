using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShelterHelper.Models;

namespace ShelterHelper.API.Controllers
{
    [Route("api/EmployeesAssignments")]
    [ApiController]
    public class EmployeesAssignmentsController : ControllerBase
    {
        private readonly ShelterContext _context;

        public EmployeesAssignmentsController(ShelterContext context)
        {
            _context = context;
        }

        // GET: api/EmployeesAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeAssignment>>> GetEmployeesAssignments()
        {
            return await _context.EmployeesAssignments.ToListAsync();
        }

        // GET: api/EmployeesAssignments/assignmentId?={assignmentId}
        [HttpGet("assignmentId")]
        public async Task<List<EmployeeAssignment>> GetEmployeesFilteredByAssignment([FromQuery] string assignmentId)
        {
            var allEmployeesAssignmentsList = await _context.EmployeesAssignments.ToListAsync();
            var filteredEmployeesList =
                allEmployeesAssignmentsList
                    .Where(assignment => assignment.AssignmentId.ToString() == assignmentId).ToList();

            return filteredEmployeesList;
        }

        // GET: api/EmployeesAssignments/employeePersonalId?={employeePersonalId}
        [HttpGet("employeePersonalId")]
        public async Task<List<EmployeeAssignment>> GetAssignmentsOfEmployee([FromQuery] string employeePersonalId)
        {
            var allEmployeesAssignmentsList = await _context.EmployeesAssignments.ToListAsync();
            var filteredAssignmentsList =
                allEmployeesAssignmentsList
                    .Where(employee => employee.Employee.EmployeePersonalId.ToString() == employeePersonalId).ToList();

            return filteredAssignmentsList;
        }

        // GET: api/EmployeesAssignments/assignmentId?={assignmentId}&employeeId?={employeeId}
        [HttpGet("query")]
        public async Task<List<EmployeeAssignment>> GetEmployeeAssignmentByAssignmentAndEmployeeIds(
            [FromQuery] string assignmentId, [FromQuery] string employeeId)
        {
            var allEmployeesAssignmentsList = await _context.EmployeesAssignments.ToListAsync();
            var filteredEmployeeAssignmentsList =
                allEmployeesAssignmentsList
                    .Where(assignment => assignment.AssignmentId.ToString() == assignmentId)
                    .Where(employee => employee.EmployeeId.ToString() == employeeId)
                    .ToList();

            return filteredEmployeeAssignmentsList;
        }

        // GET: api/EmployeesAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeAssignment>> GetEmployeesAssignments(int? id)
        {
            var employeesAssignments = await _context.EmployeesAssignments.FindAsync(id);

            if (employeesAssignments == null)
            {
                return NotFound();
            }

            return employeesAssignments;
        }

        // PUT: api/EmployeesAssignments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeesAssignments(int? id, EmployeeAssignment employeeAssignment)
        {
            if (id != employeeAssignment.Id)
            {
                return BadRequest();
            }

            _context.Entry(employeeAssignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesAssignmentsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EmployeesAssignments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmployeeAssignment>> PostEmployeesAssignments(
            EmployeeAssignment employeeAssignment)
        {
            _context.EmployeesAssignments.Add(employeeAssignment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployeesAssignments", new { id = employeeAssignment.Id }, employeeAssignment);
        }

        // DELETE: api/EmployeesAssignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAssignment(int? id)
        {
            var employeeAssignment = await _context.EmployeesAssignments.FindAsync(id);
            if (employeeAssignment == null)
            {
                return NotFound();
            }

            _context.EmployeesAssignments.Remove(employeeAssignment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EmployeesAssignments
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeesAssignmentsByEmployee(EmployeeAssignment employeeAssignment)
        {
            var employeeAssignmentToDelete = await _context.EmployeesAssignments.FindAsync(employeeAssignment);
            if (employeeAssignmentToDelete == null)
            {
                return NotFound();
            }

            _context.EmployeesAssignments.Remove(employeeAssignmentToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeesAssignmentsExists(int? id)
        {
            return _context.EmployeesAssignments.Any(e => e.Id == id);
        }
    }
}