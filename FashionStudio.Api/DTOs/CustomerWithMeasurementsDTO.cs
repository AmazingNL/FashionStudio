namespace FashionStudio.Api.DTOs
{
    public class CustomerWithMeasurementsDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ICollection<MeasurementSetDTO> MeasurementSets { get; set; } = new List<MeasurementSetDTO>();
    }
}
