using backend.DTOs.Static;

namespace backend.Services.Interfaces
{
    public interface IStaticServicecs
    {
        Task<object> GetProductSale();
        Task<IEnumerable<RevenueDtocs>> GetRevenue();
        Task<int> GetTotalByDay();
        Task<int> GetTotalOrder();
    }
}
