using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace FashionStudio.Api.Models
{
    public class WorkSpace
    {
        public WorkSpace() { }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Currency { get; set; } = "EUR";
        public Unit DefaultMeasurementUnit { get; set; } = Unit.Cm;

        public ICollection<WorkSpaceMembership> Memberships { get; set; } = new List<WorkSpaceMembership>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
