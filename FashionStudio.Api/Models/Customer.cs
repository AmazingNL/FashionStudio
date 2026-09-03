using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class Customer
    {
        public Customer() { }

        [Key]
        public int Id { get; set; }

        public WorkSpace? WorkSpace { get; set; }
        public int? WorkSpaceId { get; set; } = null;

        public User? CreatedByUser { get; set; }
        public int CreatedByUserId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PreferredContactMethod { get; set; } = string.Empty;
        public string SocialHandle { get; set; } = string.Empty;
        public string Birthday { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<MeasurementSet> MeasurementSets { get; set; } = new List<MeasurementSet>();
    }
}
