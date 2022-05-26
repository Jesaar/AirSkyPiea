using System.ComponentModel.DataAnnotations;

namespace AirSkyPiea1._0.Models
{
    public class AddDestinationImageViewModel
    {
        public int DestinationId { get; set; }

        [Display(Name = "Foto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public IFormFile ImageFile { get; set; }
    }

}
