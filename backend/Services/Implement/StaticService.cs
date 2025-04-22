using backend.Data;
using backend.DTOs.Static;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Implement
{
    public class StaticService(NhahangContext context) : IStaticServicecs
    {
        private readonly NhahangContext _context = context;

        public async Task<int> GetTotalOrder()
        {
            var totalOrder = await _context.Foodorders.CountAsync();
            return totalOrder;
        }
        public async Task<IEnumerable<RevenueDtocs>> GetRevenue()
        {
            var revenue = await _context.Foodorders.GroupBy(x => x.DeliveryTime)
                .Select(g => new RevenueDtocs
                {
                    TotalOrder = g.Count(),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                    Day = g.Key ?? DateTime.Now
                })
                .OrderBy(x=>x.Day)
                .ToListAsync();
            return revenue;
        }
        public async Task<int> GetTotalByDay()
        {
            var totalByDay = await _context.Foodorders
                .Where(x => x.DeliveryTime == DateTime.Now.Date)
                .CountAsync();
            return totalByDay;
        }
        public async Task<Object> GetProductSale()
        {
            var query = await _context.Products.Include(x=>x.Orderdetails)
                .GroupBy(g=>g.Id)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ProductName = g.FirstOrDefault().Name,
                    TotalSale = g.Sum(x => x.Orderdetails.Count),
                    Revenue = g.Sum(x => x.Orderdetails.Sum(y => y.Price * y.Quantity))
                })
                .OrderByDescending(x => x.TotalSale)
                .ToListAsync();
            return query;
        }
    }
}
