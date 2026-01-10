using Confluent.Kafka;
using KafkaTutorial.Data;
using KafkaTutorial.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

            // At least for in memory database, after dbContext.Employees.Add(employee)
            // the employee has the Id that was stored in the database, so we can return it.

            /***************** Kafka *****************************/

            // Message to publish
            var message = new Message<int, string>()
            {
                Key = employee.Id,
                Value = JsonSerializer.Serialize(employee),
            };

            // Client
            ProducerConfig producerConfig = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092",
                Acks = Acks.All
            };

            ProducerBuilder<int, string> producerBuilder = new ProducerBuilder<int, string>(producerConfig);

            using (IProducer<int, string> producer = producerBuilder.Build())
            {
                await producer.ProduceAsync("employeeTopic", message);
            }
            
            /*****************************************************/

            return CreatedAtAction(
                nameof(GetEmployee),
                new
                {
                    Id = employee.Id,
                },
                employee
                );
        }
    }
}
