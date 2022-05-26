using System.ComponentModel.DataAnnotations;

namespace AirSkyPiea1._0.Data.Entities
{
    public class SaleDetail
    {
        public int Id { get; set; }

        public Sale Sale { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentarios")]
        public string Remarks { get; set; }

        public Destination Destination { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public float Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Valor")]
        public decimal Value => Destination== null ? 0 : (decimal)Quantity * Destination.Price;
    }
}
