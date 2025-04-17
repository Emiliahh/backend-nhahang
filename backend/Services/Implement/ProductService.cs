using backend.Data;
using backend.DTOs.Category;
using backend.DTOs.Product;
using backend.Models;
using backend.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using static backend.Exceptions.ProductException;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace backend.Services.Implement
{
    public class ProductService(NhahangContext context, IValidator<ProductDto> validator, IHttpContextAccessor httpContextAccessor) : IProductService
    {
        private readonly NhahangContext _context = context;
        private readonly string _imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IValidator<ProductDto> _validator = validator;

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            return $"{request.Scheme}://{request.Host}";
        }

        public async Task<CateogryDto> CreateCategory(CateogryDto cateogryDto)
        {
            try
            {
                var existing = await _context.Categories.FirstOrDefaultAsync(x => x.Id == cateogryDto.Id);
                if (existing != null)
                {
                    throw new ValidationException("Category already exists");
                }
                var category = new Category
                {
                    Id = cateogryDto.Id,
                    Name = cateogryDto.Name
                };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return cateogryDto;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Product> CreateProduct(CreateProductDto productDto)
        {

            try
            {
                var category = await _context.Categories.FindAsync(productDto.CategoryId);
                if (category == null)
                {
                    throw new CategoryNotExistException(productDto.CategoryId);

                }
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId,
                    Description = productDto.Description ?? string.Empty
                };
                var imagePath = Path.Combine(_imageUploadPath, $"{product.Id}.jpg");

                if (productDto.Image != null)
                {
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await productDto.Image.CopyToAsync(stream);
                    }
                    product.Image = $"/uploads/{product.Id}.jpg";
                }
                else
                {
                    product.Image = "/uploads/default.jpg";
                }
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var existing = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
                if (existing == null)
                {
                    throw new ValidationException("Product not found");
                }
                existing.isDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<IEnumerable<string?>> Search(string search)
        {
            var query = _context.Products
                .Where(x => x.Name.Contains(search))
                .Include(x=>x.Orderdetails)
                .OrderByDescending(x=>x.Orderdetails.Count())
                .Select(x => x.Name);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<CartDisplayDto>> GetCartItemsAsync(IEnumerable<Guid> list)
        {
            var query = _context.Products
                 .Where(x => list.Contains(x.Id))
                 .Select(x => new CartDisplayDto
                 {
                     id = x.Id,
                     name = x.Name ?? string.Empty,
                     price = x.Price ?? 0,
                     category = x.Category.Name ?? string.Empty
                 });
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<CateogryDto>> GetCateogry()
        {
            return await _context.Categories.Select(x => new CateogryDto
            {
                Id = x.Id,
                Name = x.Name ?? string.Empty
            }).ToListAsync();
        }

        public async Task<(IEnumerable<ProductDto>,int totalPages)> GetProductsAsync(int page, int pageSize,bool desc, string? search, string? categoryId, decimal? from, decimal? to)
        {
            var query = _context.Products
                .Include(x => x.Category)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            if (from.HasValue)
            {
                query = query.Where(x => x.Price >= from.Value);
            }
            if (desc == true)
            {
                query = query.OrderByDescending(x => x.Price);
            }
            if (to.HasValue)
            {
                query = query.Where(x => x.Price <= to.Value);
            }
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var baseUrl = GetBaseUrl();
            var products= await query.
                Where(x=>x.isDeleted==false).
            Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name ?? string.Empty,
                Price = x.Price ?? 0,
                CategoryId = x.CategoryId,
                Description = x.Description,
                Image = string.IsNullOrEmpty(x.Image) ? null : $"{baseUrl}{x.Image}"
            })
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (products, totalPages);
        }

        public async Task<ProductDto?> GetProductAsync(Guid id)
        {
            var baseUrl = GetBaseUrl();

            return await _context.Products
                .Where(x => x.Id == id)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    Price = x.Price ?? 0,
                    CategoryId = x.CategoryId,
                    Description = x.Description ?? string.Empty,
                    Image = !string.IsNullOrWhiteSpace(x.Image)
                        ? $"{baseUrl}{x.Image}"
                        : null
                })
                .FirstOrDefaultAsync();
        }


        public async Task<Product> UpdateProduct(CreateProductDto pd, Guid productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId)
                          ?? throw new ProductNotFoundException(productId.ToString());
            if (product.Name != pd.Name)
                product.Name = pd.Name;

            if (product.Price != pd.Price)
                product.Price = pd.Price;

            if (product.CategoryId != pd.CategoryId)
                product.CategoryId = pd.CategoryId;

            if (product.Description != pd.Description)
                product.Description = pd.Description;

            if (pd.Image != null)
            {
                var imagePath = Path.Combine(_imageUploadPath, $"{product.Id}.jpg");
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await pd.Image.CopyToAsync(stream);
                }
                product.Image = $"/uploads/{product.Id}.jpg";
            }

            await _context.SaveChangesAsync();
            return product;
        }
    }

}
