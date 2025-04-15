using backend.DTOs.Order;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderDto> CreteOrder(CreateOrderDto dto, string userIdString);
        Task<IEnumerable<OrderDto>> GetOrders();
        Task<bool> SignPromoCode(string promoCode, Guid userId);
        Task<OrderDto> UpdateOrderStatus(Guid id, int status);
        Task<(Promo, bool status)> ValidatePromoCode(string promoCode, Guid userId);
    }
}