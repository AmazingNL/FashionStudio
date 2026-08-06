using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FashionStudio.Api.Models
{
    public class MeasurementSet
    {
        public MeasurementSet()
        {
        }
        //public MeasurementSet(int measurementSetId, Customer customer, Workspace workspace,
        //    string label, string note, Unit unit, List<MeasurementFiled> measurementFiled,
        //    DateTime dateTaken, DateTime updatedAt, User createdByUser)
        //{
        //    MeasurementSetId = measurementSetId;
        //    Customer = customer;
        //    Workspace = workspace;
        //    this.label = label;
        //    Note = note;
        //    Unit = unit;
        //    MeasurementFiled = measurementFiled;
        //    DateTaken = dateTaken;
        //    UpdatedAt = updatedAt;
        //    CreatedByUser = createdByUser;
        //}
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int WorkSpaceId { get; set; }
        public WorkSpace? WorkSpace { get; set; }

        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public string label { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public Unit Unit { get; set; } = Unit.Cm;
        public DateTime DateTaken { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<MeasurementFiled> MeasurementFiled { get; set; } = new List<MeasurementFiled>();

    }
}
