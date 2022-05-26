using AirSkyPiea1._0.Common;
using AirSkyPiea1._0.Data.Entities;

namespace AirSkyPiea1._0.Models
{
    public class HomeViewModel
    {
        public PaginatedList<Destination> Destinations { get; set; }

        public ICollection<Category> Categories { get; set; }

        public float Quantity { get; set; }
    }
}
