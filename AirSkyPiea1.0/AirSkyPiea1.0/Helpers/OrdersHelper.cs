using Microsoft.EntityFrameworkCore;
using AirSkyPiea1._0.Common;
using AirSkyPiea1._0.Data;
using AirSkyPiea1._0.Data.Entities;
using AirSkyPiea1._0.Enums;
using AirSkyPiea1._0.Models;

namespace AirSkyPiea1._0.Helpers
{
    public class OrdersHelper : IOrdersHelper
    {
        private readonly DataContext _context;

        public OrdersHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<Response> ProcessOrderAsync(ShowCartViewModel model)
        {
            Response response = await CheckInventoryAsync(model);
            if (!response.IsSuccess)
            {
                return response;
            }

            Sale sale = new()
            {
                Date = DateTime.UtcNow,
                User = model.User,
                Remarks = model.Remarks,
                SaleDetails = new List<SaleDetail>(),
                OrderStatus = OrderStatus.Nuevo
            };

            foreach (TemporalSale item in model.TemporalSales)
            {
                sale.SaleDetails.Add(new SaleDetail
                {
                    Destination = item.Destination,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                });

                Destination destination = await _context.Destinations.FindAsync(item.Destination.Id);
                if (destination!= null)
                {
                    destination.Stock -= item.Quantity;
                    _context.Destinations.Update(destination);
                }

                _context.TemporalSales.Remove(item);
            }

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<Response> CancelOrderAsync(int id)
        {
            Sale sale = await _context.Sales
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Destination)
                .FirstOrDefaultAsync(s => s.Id == id);

            foreach (SaleDetail saleDetail in sale.SaleDetails)
            {
                Destination product = await _context.Destinations.FindAsync(saleDetail.Destination.Id);
                if (product != null)
                {
                    product.Stock += saleDetail.Quantity;
                }
            }

            sale.OrderStatus = OrderStatus.Cancelado;
            await _context.SaveChangesAsync();
            return new Response { IsSuccess = true };
        }

        private async Task<Response> CheckInventoryAsync(ShowCartViewModel model)
        {
            Response response = new() { IsSuccess = true };
            foreach (TemporalSale item in model.TemporalSales)
            {
                Destination product = await _context.Destinations.FindAsync(item.Destination.Id);
                if (product == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"El producto {item.Destination.Name}, ya no está disponible";
                    return response;
                }
                if (product.Stock < item.Quantity)
                {
                    response.IsSuccess = false;
                    response.Message = $"Lo sentimos no tenemos existencias suficientes del producto {item.Destination.Name}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.";
                    return response;
                }
            }
            return response;
        }
    }
}
