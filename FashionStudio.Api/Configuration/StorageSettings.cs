namespace FashionStudio.Api.Configuration
{
    public class StorageSettings
    {
        public string OrderImagesPath { get; set; } = "App_Data/OrderImages";
        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
    }
}
