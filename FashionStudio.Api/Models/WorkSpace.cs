using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class WorkSpace
    {
        public WorkSpace() { }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int OwnerId { get; set; }   
        public User? Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Currency { get; set; } = "EUR";
        public Unit DefaultMeasurementUnit { get; set; } = Unit.Cm;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
