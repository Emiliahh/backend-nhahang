using backend.DTOs.Category;
using backend.DTOs.Product;
using backend.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto>,int totalPages)> GetProductsAsync(int page,int pageSize,bool desc, string? search,string ? categoryId,float ?from, float ?to);
        Task<IEnumerable<CartDisplayDto>> GetCartItemsAsync(IEnumerable<string> list);
        Task<ProductDto> CreateProduct(ProductDto productDto);
        Task<CateogryDto> CreateCategory(CateogryDto cateogryDto);
        Task<IEnumerable<CateogryDto>> GetCateogry();
        Task<bool> Delete(string id);
        Task<Product> UpdateProduct(ProductDto pd);
        //Task<ProductDto> UpdateProduct(ProductDto productDto);
    }
}
