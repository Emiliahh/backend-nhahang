using backend.Data;
using backend.DTOs.Product;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static backend.Exceptions.ProductException;

namespace backend.Services.Implement
{
    public class CartService : ICartService
    {
        private readonly NhahangContext _context;
        public CartService(NhahangContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<CartItemDto>> getCartItem(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    throw new ArgumentException("Invalid user ID format.", nameof(userId));
                }
                var res = await _context.Cartitems.Where(x => x.UserId.Equals(parsedUserId)).Select(x => new CartItemDto
                {
                    id = x.ProductId,
                    quantity = x.Quantity,
                    note = x.Note,
                }).ToListAsync();
                return res;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Cartitem> addProduct(string userId, CartItemDto cart)
        {
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                throw new ArgumentException("Invalid user ID format.", nameof(userId));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existing = await _context.Cartitems
                    .FirstOrDefaultAsync(x => x.ProductId == cart.id && x.UserId == parsedUserId);

                if (existing != null)
                {
                    existing.Quantity += cart.quantity;
                    existing.Note = cart.note;
                    _context.Cartitems.Update(existing);
                }
                else
                {
                    var newItem = new Cartitem
                    {
                        UserId = parsedUserId,
                        ProductId = cart.id,
                        Quantity = cart.quantity,
                        Note = cart.note
                    };
                    await _context.Cartitems.AddAsync(newItem);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return existing ?? new Cartitem
                {
                    UserId = parsedUserId,
                    ProductId = cart.id,
                    Quantity = cart.quantity,
                    Note = cart.note
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> Remove(string userId, Guid productId)
        {
            try
            {
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    throw new ArgumentException("Invalid user ID format.", nameof(userId));
                }
                var existing = await _context.Cartitems.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == parsedUserId);
                if (existing == null)
                {
                    return false;
                }
                _context.Cartitems.Remove(existing);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Cartitem> updateQuantity(string userId, Guid productId, int quantity)
        {
            try
            {
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    throw new ArgumentException("Invalid user ID format.", nameof(userId));
                }
                var existing = await _context.Cartitems.FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == parsedUserId);
                if (existing == null)
                {
                    throw new ProductNotFoundException(productId.ToString());
                }
                existing.Quantity = quantity;
                _context.Cartitems.Update(existing);
                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }

}
