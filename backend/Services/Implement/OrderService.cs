using backend.Data;
using backend.DTOs.Order;
using backend.Models;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implement
{
    public class OrderService : IOrderService
    {
        private readonly NhahangContext _context;

        public OrderService(NhahangContext context)
        {
            _context = context;
        }

        // Create new order 
        public async Task<(Promo, bool status)> ValidatePromoCode(string promoCode, Guid userId)
        {
            try
            {
                var code = await _context.Promos.FirstOrDefaultAsync(x => x.Code == promoCode);
                var usage = await _context.PromoUsages.FirstOrDefaultAsync(_context => _context.UserId == userId && _context.PromoId == code.Id);
                if (code == null || code.ExpirationDate < DateTime.Now || code.UsageCount >= code.MaxUsage || usage != null)
                {
                    return (null, false);
                }
                return (code, true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        // sign promo code is usage

        public async Task<bool> SignPromoCode(string promoCode, Guid userId)
        {
            try
            {
                var code = await _context.Promos.FirstOrDefaultAsync(x => x.Code == promoCode);
                if (code == null)
                {
                    return false;
                }
                var usage = new PromoUsage
                {
                    UserId = userId,
                    PromoId = code.Id,
                    UsedAt = DateTime.Now
                };
                _context.PromoUsages.Add(usage);
                code.UsageCount++;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<(Guid orderId, decimal totalPrice)> CreteOrder(CreateOrderDto dto, string userIdString)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!Guid.TryParse(userIdString, out Guid userId))
                {
                    throw new Exception("Invalid user ID.");
                }

                var order = new Order
                {
                    PaymentMethod = dto.PaymentMethod,
                    Address = dto.Address,
                    DeliveryFee = dto.DeliveryFee,
                    Note = dto.Note,
                    OrderTime = DateTime.Now,
                    Orderdetails = [],
                    IsPaid = false,
                };

                if (dto.PromoCode != null)
                {
                    var (promo, status) = await ValidatePromoCode(dto.PromoCode, userId);
                    if (!status)
                    {
                        throw new Exception("Promo code is not valid.");
                    }

                    if (promo.IsPercentage)
                    {
                        var discountValue = (order.DeliveryFee + order.TotalPrice) / 100 * promo.DiscountValue;
                        order.Discount = Math.Min(discountValue, promo.MaxDiscountAmount ?? 0);
                    }
                    else
                    {
                        order.Discount = promo.DiscountValue;
                    }

                    var usage = await SignPromoCode(dto.PromoCode, userId);
                    if (!usage)
                    {
                        throw new Exception("Promo code is not valid.");
                    }
                }

                order.UserId = userId;
                order.Status = OrderStatus.Pending;

                foreach (var orderDetailDto in dto.CreateOrderDetailDtos)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderDetailDto.ProductId);
                    if (product == null)
                    {
                        throw new Exception("Product not found.");
                    }

                    var orderDetail = new Orderdetail
                    {
                        OrderId = order.Id,
                        ProductId = orderDetailDto.ProductId,
                        ProductName = product.Name ?? "",
                        Price = product.Price ?? 0,
                        Quantity = orderDetailDto.Quantity,
                        Note = orderDetailDto.Note
                    };
                    order.Orderdetails.Add(orderDetail);
                }
                order.ProductTotal = order.Orderdetails.Sum(x => x.Price * x.Quantity);
                order.TotalPrice = order.ProductTotal + order.DeliveryFee - (order.Discount ?? 0);
                _context.Foodorders.Add(order);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (order.Id, order.TotalPrice);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error occurred while creating order: " + e.Message);
            }
        }
        public async Task<(IEnumerable<OrderDto>, int totalPage)> GetOrders(int status, int page, int pagesize)
        {
            try
            {
                var queryable = _context.Foodorders
                    .Include(x => x.Orderdetails)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.OrderTime)
                    .AsQueryable();

                if (status != 0)
                {
                    queryable = queryable.Where(x => x.Status == (OrderStatus)status);
                }
                int totalItem = await queryable.CountAsync();
                int totalPage = (int)Math.Ceiling((double)totalItem / pagesize);

                var result = await queryable
                    .Skip((page - 1) * pagesize)
                    .Take(pagesize)
                    .Select(x => new OrderDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Address = x.Address,
                        DeliveryFee = x.DeliveryFee,
                        Note = x.Note,
                        OrderTime = x.OrderTime,
                        PaymentMethod = x.PaymentMethod,
                        Status = x.Status,
                        TotalPrice = x.TotalPrice,
                        Discount = x.Discount,
                        DeliveryTime = x.DeliveryTime,
                        CustomerName = x.User != null ? x.User.Email ?? "" : "",
                        IsPaid = x.IsPaid,
                        OrderDetails = x.Orderdetails.Select(y => new OrderDetailDto
                        {
                            ProductId = y.ProductId,
                            ProductName = y.ProductName,
                            Price = y.Price,
                            Quantity = y.Quantity,
                            Note = y.Note
                        }).ToList()
                    }).ToListAsync();


                return (result, totalPage);

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<OrderDto> UpdateOrderStatus(Guid id, int status)
        {
            try
            {
                if (!Enum.IsDefined(typeof(OrderStatus), status))
                {
                    throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
                }
                var order = _context.Foodorders.FirstOrDefault(x => x.Id == id);
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }
                var pStatus = (OrderStatus)status;

                if (pStatus == OrderStatus.Cancel && order.Status == OrderStatus.Delivered)
                {
                    throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
                }
                if (pStatus == OrderStatus.Delivering && order.Status != OrderStatus.Pending)
                {
                    throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
                }
                order.Status = pStatus;

                if (pStatus == OrderStatus.Delivered && order.PaymentMethod == PaymentMethodType.Cash)
                {
                    order.DeliveryTime = DateTime.Now;
                    order.IsPaid = true;
                }
                else if (pStatus == OrderStatus.Cancel)
                {
                    order.DeliveryTime = null;
                }

                _context.SaveChanges();
                return Task.FromResult(new OrderDto());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
        public Task<bool> UpdatePaymentStatus(Guid id)
        {
            try
            {
                var order = _context.Foodorders.FirstOrDefault(x => x.Id == id);
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }
                order.IsPaid = true;
                order.DeliveryTime = DateTime.Now;
                _context.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<IEnumerable<OrderDto>> GetOrdersByUser(Guid id, OrderStatus status)
        {
            try
            {

                var query = await _context.Foodorders
                    .Include(x => x.Orderdetails)
                    .Include(x => x.User)
                    .Where(x => x.UserId == id && x.Status == status)
                    .OrderByDescending(x => x.OrderTime)
                    .Select(x => new OrderDto
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Address = x.Address,
                        DeliveryFee = x.DeliveryFee,
                        Note = x.Note,
                        OrderTime = x.OrderTime,
                        PaymentMethod = x.PaymentMethod,
                        Status = x.Status,
                        TotalPrice = x.TotalPrice,
                        Discount = x.Discount,
                        DeliveryTime = x.DeliveryTime,
                        InternalNote = x.InternalNote,
                        CustomerName = x.User != null ? x.User.Email ?? "" : "",
                        IsPaid = x.IsPaid,
                        OrderDetails = x.Orderdetails.Select(y => new OrderDetailDto
                        {
                            ProductId = y.ProductId,
                            ProductName = y.ProductName,
                            Price = y.Price,
                            Quantity = y.Quantity,
                            Note = y.Note
                        }).ToList()
                    }).ToListAsync();

                return query;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public Task<bool> UserCancelOrder(Guid orderId,Guid userId)
        {
            try
            {
                var order = _context.Foodorders.FirstOrDefault(x => x.Id == orderId && x.UserId == userId);
                if (order == null)
                {
                    throw new Exception("Order not found.");
                }
                if (order.Status != OrderStatus.Pending)
                {
                    throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
                }
                if(order.Status != OrderStatus.Pending)
                {
                    throw new ArgumentException("Trạng thái đơn hàng không hợp lệ.");
                }
                var note = "Đã huỷ đơn hàng";
                if (order.IsPaid)
                {
                    order.IsPaid = false;
                    order.DeliveryTime = null;
                    note = "Đã huỷ đơn hàng và hoàn tiền";
                }
                order.Status = OrderStatus.Cancel;
                order.DeliveryTime = null;
                order.InternalNote = note;
                _context.SaveChanges();
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}
