using KafkaTutorial.Data;
using KafkaTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KafkaTutorial.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EmployeesController(AppDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {

            IEnumerable<Employee> employees = await dbContext.Employees.ToListAsync();

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee?>> GetEmployee(int id)
        {
            Employee? employee = await dbContext.FindAsync<Employee>(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost("{name}/{surname}")]
        public async Task<ActionResult> CreateEmployee(string name, string surname)
        {
            Employee employee = new Employee
            {
                Name = name,
                Surname = surname
            };

            dbContext.Employees.Add(employee);
            await dbContext.SaveChangesAsync();

            return Created();
        }
    }
}
