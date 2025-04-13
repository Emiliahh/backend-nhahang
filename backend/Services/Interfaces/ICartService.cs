using backend.DTOs.Product;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cartitem> addProduct(string userId, CartItemDto cart);
        Task<IEnumerable<CartItemDto>> getCartItem(string userId);
        Task<bool> Remove(string userId, Guid productId);
        Task<Cartitem> updateQuantity(string userId, Guid productId, int quantity);
    }
}