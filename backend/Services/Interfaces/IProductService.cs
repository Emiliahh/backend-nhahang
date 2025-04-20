using backend.DTOs.Category;
using backend.DTOs.Product;
using backend.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace backend.Services.Interfaces
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto>,int totalPages)> GetProductsAsync(int page,int pageSize,bool desc, string? search,string ? categoryId,decimal ?from, decimal ?to);
        Task<IEnumerable<CartDisplayDto>> GetCartItemsAsync(IEnumerable<Guid> list);
        Task<Product> CreateProduct(CreateProductDto productDto);
        Task<CateogryDto> CreateCategory(CateogryDto cateogryDto);
        Task<IEnumerable<CateogryDto>> GetCateogry();
        Task<bool> Delete(Guid id);
        Task<Product> UpdateProduct(CreateProductDto pd,Guid productId);
        Task<ProductDto?> GetProductAsync(Guid id);
        Task<IEnumerable<string?>> Search(string search);
        Task<IEnumerable<ProductDto>> SearchProducts(IEnumerable<Guid> List);
        //Task<ProductDto> UpdateProduct(ProductDto productDto);
    }
}
