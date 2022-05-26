using AirSkyPiea1._0.Common;
using AirSkyPiea1._0.Models;

namespace AirSkyPiea1._0.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response> ProcessOrderAsync(ShowCartViewModel model);

        Task<Response> CancelOrderAsync(int id);
    }
}


