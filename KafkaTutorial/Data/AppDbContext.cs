using KafkaTutorial.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaTutorial.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }
    }
}
