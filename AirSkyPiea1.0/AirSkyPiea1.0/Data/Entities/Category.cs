using System.ComponentModel.DataAnnotations;

namespace AirSkyPiea1._0.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Categoría")]
        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El Campo {0} es obligatorio.")]
        public string Name { get; set; }

        public ICollection<DestinationCategory> DestinationCategories { get; set; }

        [Display(Name = "# Productos")]
        public int DestinationNumber => DestinationCategories == null ? 0 : DestinationCategories.Count();
    }
}
