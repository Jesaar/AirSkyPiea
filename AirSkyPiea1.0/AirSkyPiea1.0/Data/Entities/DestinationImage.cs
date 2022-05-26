using System.ComponentModel.DataAnnotations;

namespace AirSkyPiea1._0.Data.Entities
{
    public class DestinationImage
    {
        public int Id { get; set; }

        public Destination Destination { get; set; }

        [Display(Name = "Foto")]
        public Guid ImageId { get; set; }

        [Display(Name = "Foto")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://zulushooping.azurewebsites.net/images/noimage.png"
            : $"https://shoppingzulu.blob.core.windows.net/products/{ImageId}";
    }
}
