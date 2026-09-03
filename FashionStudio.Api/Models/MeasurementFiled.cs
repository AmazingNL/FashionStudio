using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace FashionStudio.Api.Models
{
    public class MeasurementFiled
    {
        public MeasurementFiled()
        {
        }

        [Key]
        public int Id { get; set; }

        public int MeasurementSetId { get; set; }      // FK
        public MeasurementSet? MeasurementSet { get; set; }  // Navigation

        public decimal Height { get; set; }
        public decimal Bust { get; set; }
        public decimal UnderBust { get; set; }
        public decimal Waist { get; set; }
        public decimal Hip { get; set; }
        public decimal ShoulderWidth { get; set; }
        public decimal Neck { get; set; }
        public decimal SleeveLength { get; set; }
        public decimal Armhole { get; set; }
        public decimal Bicep { get; set; }
        public decimal TopLength { get; set; }
        public decimal Dresslength { get; set; }
        public decimal SkirtLength { get; set; }
        public decimal Inseam { get; set; }
        public decimal Outseam { get; set; }
        public decimal Thigh { get; set; }
        public decimal Knee { get; set; }
        public decimal Ankle { get; set; }
        public string CustomMeasurementJson { get; set; } = "{}";


        [NotMapped]
        public Dictionary<string, decimal> CustomMeasurements 
        { get =>JsonSerializer.Deserialize<Dictionary<string, decimal>>(CustomMeasurementJson)
                ?? new Dictionary<string, decimal>();
            set => CustomMeasurementJson = JsonSerializer.Serialize(value); }

    }
}
