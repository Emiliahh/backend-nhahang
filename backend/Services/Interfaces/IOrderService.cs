using backend.DTOs.Order;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IOrderService
    {
        Task<(Guid orderId,decimal totalPrice)> CreteOrder(CreateOrderDto dto, string userIdString);
        Task<(IEnumerable<OrderDto>, int totalPage)> GetOrders(int status, int page, int pagesize);
        Task<IEnumerable<OrderDto>> GetOrdersByUser(Guid id, OrderStatus status);
        Task<bool> SignPromoCode(string promoCode, Guid userId);
        Task<OrderDto> UpdateOrderStatus(Guid id, int status);
        Task<bool> UpdatePaymentStatus(Guid id);
        Task<bool> UserCancelOrder(Guid orderId, Guid userId);
        Task<(Promo, bool status)> ValidatePromoCode(string promoCode, Guid userId);
    }
}