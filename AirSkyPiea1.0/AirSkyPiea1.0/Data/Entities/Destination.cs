using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirSkyPiea1._0.Data.Entities
{
    public class Destination
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción")]
        [MaxLength(500, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public float Stock { get; set; }

        public ICollection<DestinationCategory> DestinationCategories { get; set; }

        [Display(Name = "Categorías")]
        public int CategoriesNumber => DestinationCategories == null ? 0 : DestinationCategories.Count;

        public ICollection<DestinationImage> DestinationImages { get; set; }

        [Display(Name = "Fotos")]
        public int ImagesNumber => DestinationImages == null ? 0 : DestinationImages.Count;

        [Display(Name = "Foto")]
        public string ImageFullPath => DestinationImages == null || DestinationImages.Count == 0
            ? $"https://zulushooping.azurewebsites.net/images/noimage.png"
            : DestinationImages.FirstOrDefault().ImageFullPath;

        public ICollection<SaleDetail> SaleDetails { get; set; }
    }
}
