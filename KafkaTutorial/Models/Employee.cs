using System.ComponentModel.DataAnnotations;

namespace KafkaTutorial.Models
{
    public record Employee
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
    }
}
